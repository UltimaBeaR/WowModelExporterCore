#include "stdafx.h"

#include <fbxsdk.h>
#include <fbxsdk\fileio\fbxiosettings.h>

#include "Exporter.h"

using namespace System;
using namespace System::Drawing;
using namespace System::Drawing::Imaging;
using namespace System::Collections::Generic;
using namespace System::IO;
using namespace System::Runtime::InteropServices;

char* GetString(String^ csString)
{
	return (char*)(void*)Marshal::StringToHGlobalAnsi(csString);
}

FbxVector4 ConvertPositionFromWowVec3(float x, float y, float z, float w = 1.0)
{
	// Флипаем x, настройками сцены это сделать не получилось
	return FbxVector4(-x, y, z, w);
}

String^ GetRelativeTextureFileName(TextureImage^ textureImage)
{
	return "Textures/" + textureImage->Hash + ".png";
}

WowModelExporterFbx::Exporter::Exporter()
{
	_manager = nullptr;
	_textureFiles = gcnew HashSet<TextureImage^>;
}

WowModelExporterFbx::Exporter::~Exporter()
{
	if (_manager != nullptr)
	{
		_manager->Destroy();
		_manager = nullptr;
	}
}

Boolean WowModelExporterFbx::Exporter::ExportWowObject(WowObject^ characterWowObject, String^ exportDirectory)
{
	if (characterWowObject == nullptr)
		throw gcnew ArgumentException("characterWowObject");

	if (!Directory::Exists(exportDirectory))
		throw gcnew DirectoryNotFoundException(exportDirectory);

	if (_manager != nullptr)
		_manager->Destroy();
	_manager = FbxManager::Create();

	_scene = nullptr;

	_textureFiles->Clear();

	auto ioSettings = FbxIOSettings::Create(_manager, IOSROOT);
	_manager->SetIOSettings(ioSettings);

	CreateScene();

	_characterWowObject = characterWowObject;

	CreateCharacter();

	auto exported = ExportSceneToFbxFile(exportDirectory);

	_manager->Destroy();
	_manager = nullptr;

	return Boolean(exported);
}

void WowModelExporterFbx::Exporter::CreateScene()
{
	_scene = FbxScene::Create(_manager, "wowmodelexporter");

	FbxSystemUnit::m.ConvertScene(_scene);
	FbxAxisSystem::MayaYUp.ConvertScene(_scene);
}

void WowModelExporterFbx::Exporter::CreateCharacter()
{
	_skeletonNodesArray = new FbxSkeleton*[_characterWowObject->Bones->Length];
	for (int i = 0; i < _characterWowObject->Bones->Length; i++)
		_skeletonNodesArray[i] = NULL;

	auto armatureNode = FbxNode::Create(_scene, "Armature");

	CreateCharacterSkeleton(armatureNode);

	CreateAllSkinnedMeshesForCharacter(armatureNode);

	_scene->GetRootNode()->AddChild(armatureNode);

	delete[] _skeletonNodesArray;
}

bool WowModelExporterFbx::Exporter::ExportSceneToFbxFile(String^ exportDirectory)
{
	for each (TextureImage^ img in _textureFiles)
	{
		auto relativeFileName = GetRelativeTextureFileName(img);
		auto bitmap = img->Bitmap;

		auto absoluteFileName = Path::Combine(exportDirectory, relativeFileName);

		(gcnew FileInfo(absoluteFileName))->Directory->Create();

		bitmap->Save(absoluteFileName, ImageFormat::Png);
	}

	auto fbxFileName = GetString(Path::Combine(exportDirectory, "character.fbx"));

	auto exporter = FbxExporter::Create(_manager, "");
	auto exporterStatus = exporter->Initialize(fbxFileName, -1, _manager->GetIOSettings());

	return exporter->Export(_scene);
}

void WowModelExporterFbx::Exporter::CreateCharacterSkeleton(FbxNode* node)
{
	FbxSkeleton* skeletonRoot = FbxSkeleton::Create(_scene, "skeleton_root");
	skeletonRoot->SetSkeletonType(FbxSkeleton::eRoot);
	node->SetNodeAttribute(skeletonRoot);

	_rootSkeleton = skeletonRoot;

	auto wowRootBone = _characterWowObject->GetRootBone();

	if (wowRootBone == nullptr)
		return;

	CreateChildBones(node, wowRootBone);
}

FbxNode* WowModelExporterFbx::Exporter::CreateChildBones(FbxNode* parentNode, WowBone^ wowBone)
{
	auto boneNode = CreateChildBone(parentNode, wowBone);

	for (int i = 0; i < wowBone->ChildBones->Count; i++)
		CreateChildBones(boneNode, wowBone->ChildBones[i]);

	return boneNode;
}

FbxNode* WowModelExporterFbx::Exporter::CreateChildBone(FbxNode* parentNode, WowBone^ wowBone)
{
	auto wowBoneName = wowBone->GetName();
	auto boneName = GetString(wowBoneName != nullptr ? wowBoneName : gcnew String("bone ") + wowBone->Id);

	auto boneNode = FbxNode::Create(_scene, boneName);

	FbxSkeleton* skeleton = FbxSkeleton::Create(_scene, boneName);
	skeleton->SetSkeletonType(FbxSkeleton::eLimbNode);
	skeleton->Size.Set(1.0);
	boneNode->SetNodeAttribute(skeleton);

	_skeletonNodesArray[wowBone->Index] = skeleton;

	boneNode->LclTranslation.Set(ConvertPositionFromWowVec3(wowBone->LocalPosition.X, wowBone->LocalPosition.Y, wowBone->LocalPosition.Z));

	parentNode->AddChild(boneNode);

	return boneNode;
}

void WowModelExporterFbx::Exporter::CreateAllSkinnedMeshesForCharacter(FbxNode* node)
{
	// создаем материалы, меш и применяем скин для самого персонажа (меш Body)

	auto characterMeshNode = FbxNode::Create(_scene, "Body");

	node->AddChild(characterMeshNode);

	CreateMaterialsPerWowSubmesh(characterMeshNode, _characterWowObject->Mesh, GetString(gcnew String("character_")));

	auto characterMesh = CreateMesh(characterMeshNode, _characterWowObject->Mesh);

	ApplySkinForCharacterMesh(_characterWowObject, characterMesh);

	// создаем материалы, меш и применяем скин для прикрепленных к костям предметов

	for (int boneIdx = 0; boneIdx < _characterWowObject->Bones->Length; boneIdx++)
	{
		auto wowBone = _characterWowObject->Bones[boneIdx];

		if (wowBone != nullptr && wowBone->AttachedWowObjects->Count > 0)
		{
			for (int attachedIdx = 0; attachedIdx < wowBone->AttachedWowObjects->Count; attachedIdx++)
			{
				WowObject^ attachedWowObject = wowBone->AttachedWowObjects[attachedIdx];

				auto attachedObjectMeshNodeName = wowBone->GetName() + "_" + Convert::ToString(attachedIdx);

				auto attachedObjectMeshNode = FbxNode::Create(_scene, GetString(attachedObjectMeshNodeName));

				node->AddChild(attachedObjectMeshNode);

				auto globalBonePos = wowBone->GetGlobalPosition();
				attachedObjectMeshNode->LclTranslation.Set(ConvertPositionFromWowVec3(globalBonePos.X, globalBonePos.Y, globalBonePos.Z));

				CreateMaterialsPerWowSubmesh(attachedObjectMeshNode, attachedWowObject->Mesh, GetString(gcnew String("") + wowBone->Id + "_" + Convert::ToString(attachedIdx) + "_"));

				auto attachedObjectMesh = CreateMesh(attachedObjectMeshNode, attachedWowObject->Mesh);

				ApplySkinForAttachedObjectMesh(wowBone, attachedObjectMesh);
			}
		}
	}
}

void WowModelExporterFbx::Exporter::CreateMaterialsPerWowSubmesh(FbxNode* node, WowMeshWithMaterials^ wowMesh, char* fileNamePrefix)
{
	for (int submeshIdx = 0; submeshIdx < wowMesh->Submeshes->Count; submeshIdx++)
	{
		auto wowMaterial = wowMesh->Submeshes[submeshIdx]->Material;

		auto materialName = FbxString("material submesh[") + submeshIdx + "]";
		auto textureName = FbxString("main texture submesh[") + submeshIdx + "]";
		
		if (!_textureFiles->Contains(wowMaterial->MainImage))
			_textureFiles->Add(wowMaterial->MainImage);

		FbxSurfacePhong* material = FbxSurfacePhong::Create(_scene, materialName.Buffer());

		material->ShadingModel.Set(FbxString("Phong"));

		// ToDo: у материала в блендере стоит галочка transparency. надо ее убрать. точнее сделать ее только там где материал действительно полупрозрачный

		material->TransparencyFactor.Set(0);

		material->Diffuse.Set(FbxDouble3(1, 1, 1));
		material->DiffuseFactor.Set(1);

		material->Ambient.Set(FbxDouble3(1, 1, 1));
		material->AmbientFactor.Set(0);

		material->Emissive.Set(FbxDouble3(0, 0, 0));
		material->EmissiveFactor.Set(0);

		material->Specular.Set(FbxDouble3(1, 1, 1));
		material->SpecularFactor.Set(0);

		material->Shininess.Set(0);

		node->AddMaterial(material);

		auto texture = FbxFileTexture::Create(_scene, textureName.Buffer());

		texture->SetFileName(GetString(GetRelativeTextureFileName(wowMaterial->MainImage)));
		texture->SetTextureUse(FbxTexture::eStandard);
		texture->SetMappingType(FbxTexture::eUV);
		texture->SetMaterialUse(FbxFileTexture::eModelMaterial);
		texture->SetSwapUV(false);
		texture->SetTranslation(0.0, 0.0);
		texture->SetScale(1.0, 1.0);
		texture->SetRotation(0.0, 0.0);

		material->Diffuse.ConnectSrcObject(texture);
	}
}

FbxMesh* WowModelExporterFbx::Exporter::CreateMesh(FbxNode* node, WowMeshWithMaterials^ wowMesh)
{
	auto mesh = FbxMesh::Create(_scene, "mesh");
	node->SetNodeAttribute(mesh);

	// вершины

	mesh->InitControlPoints(wowMesh->Vertices->Length);
	FbxVector4* vertices = mesh->GetControlPoints();

	for (int i = 0; i < wowMesh->Vertices->Length; i++)
	{
		auto wowVertex = wowMesh->Vertices[i];
		vertices[i] = ConvertPositionFromWowVec3(wowVertex->Position.X, wowVertex->Position.Y, wowVertex->Position.Z);
	}

	// нормали

	FbxGeometryElementNormal* normalElement = mesh->CreateElementNormal();
	normalElement->SetMappingMode(FbxGeometryElement::eByControlPoint);
	normalElement->SetReferenceMode(FbxGeometryElement::eDirect);

	for (int i = 0; i < wowMesh->Vertices->Length; i++)
	{
		auto wowVertex = wowMesh->Vertices[i];
		normalElement->GetDirectArray().Add(ConvertPositionFromWowVec3(wowVertex->Normal.X, wowVertex->Normal.Y, wowVertex->Normal.Z, 0));
	}

	// uv1 координаты

	FbxGeometryElementUV* uv1Element = mesh->CreateElementUV("UV1");
	uv1Element->SetMappingMode(FbxGeometryElement::eByControlPoint);
	uv1Element->SetReferenceMode(FbxGeometryElement::eDirect);

	for (int i = 0; i < wowMesh->Vertices->Length; i++)
	{
		auto wowVertex = wowMesh->Vertices[i];
		uv1Element->GetDirectArray().Add(FbxVector2(wowVertex->UV1.X, wowVertex->UV1.Y));
	}

	// ToDo: если второй сет поставить то почему-то вообще перестает отображать текстуры
	// uv2 координаты

	//FbxGeometryElementUV* uv2Element = mesh->CreateElementUV("UV2");
	//uv2Element->SetMappingMode(FbxGeometryElement::eByControlPoint);
	//uv2Element->SetReferenceMode(FbxGeometryElement::eDirect);

	//for (int i = 0; i < wowMesh->Vertices->Length; i++)
	//{
	//	auto wowVertex = wowMesh->Vertices[i];
	//	uv2Element->GetDirectArray().Add(FbxVector2(wowVertex->UV2.X, wowVertex->UV2.Y));
	//}

	// треугольники (на каждый сабмеш)

	FbxGeometryElementMaterial* materialElement = mesh->CreateElementMaterial();
	materialElement->SetMappingMode(FbxGeometryElement::eByPolygon);
	materialElement->SetReferenceMode(FbxGeometryElement::eIndexToDirect);

	materialElement->GetIndexArray().Add(0);

	for (int submeshIdx = 0; submeshIdx < wowMesh->Submeshes->Count; submeshIdx++)
	{
		auto wowSubmesh = wowMesh->Submeshes[submeshIdx];

		for (int triangleIdx = 0; triangleIdx < wowSubmesh->Triangles->Length; triangleIdx += 3)
		{
			mesh->BeginPolygon(submeshIdx);

			// (флипаем порядок отрисовки треугольников, т.к. флипали X позиции)
			mesh->AddPolygon(wowSubmesh->Triangles[triangleIdx + 2]);
			mesh->AddPolygon(wowSubmesh->Triangles[triangleIdx + 1]);
			mesh->AddPolygon(wowSubmesh->Triangles[triangleIdx]);

			mesh->EndPolygon();
		}
	}

	return mesh;
}

array<List<Tuple<Int32, Single>^>^>^ WowModelExporterFbx::Exporter::CreateBonesInfluenceOnCharacterMeshVertices(WowObject^ wowObject)
{
	auto influenceOnVertices = gcnew array<List<Tuple<Int32, Single>^>^>(wowObject->Bones->Length);

	auto vertices = wowObject->Mesh->Vertices;

	for (int vertexIdx = 0; vertexIdx < vertices->Length; vertexIdx++)
	{
		auto vertex = vertices[vertexIdx];

		for (int boneInVertexIdx = 0; boneInVertexIdx < 4; boneInVertexIdx++)
		{
			Single boneWeight = vertex->BoneWeights[boneInVertexIdx];

			if (boneWeight > 0.0f)
			{
				Byte boneIndex = vertex->BoneIndexes[boneInVertexIdx];

				if (influenceOnVertices[boneIndex] == nullptr)
					influenceOnVertices[boneIndex] = gcnew List<Tuple<Int32, Single>^>(1);

				influenceOnVertices[boneIndex]->Add(gcnew Tuple<Int32, Single>(vertexIdx, boneWeight));
			}
		}
	}

	return influenceOnVertices;
}

void WowModelExporterFbx::Exporter::ApplySkinForCharacterMesh(WowObject^ wowObject, FbxMesh* characterMesh)
{
	auto characterMeshGlobalMatrix = FbxAMatrix(characterMesh->GetNode()->EvaluateGlobalTransform());

	auto bonesInfluenceOnCharacterMeshVertices = CreateBonesInfluenceOnCharacterMeshVertices(wowObject);

	FbxSkin* characterSkin = FbxSkin::Create(_scene, "");

	// root skeleton -> character mesh

	auto rootSkeletonNode = _rootSkeleton->GetNode();

	FbxCluster* rootSkeletonCluster = FbxCluster::Create(_scene, "");
	rootSkeletonCluster->SetLink(rootSkeletonNode);
	rootSkeletonCluster->SetLinkMode(FbxCluster::eTotalOne);

	auto rootSkeletonNodeGlobalMatrix = rootSkeletonNode->EvaluateGlobalTransform();
	rootSkeletonCluster->SetTransformMatrix(characterMeshGlobalMatrix);
	rootSkeletonCluster->SetTransformLinkMatrix(rootSkeletonNodeGlobalMatrix);

	characterSkin->AddCluster(rootSkeletonCluster);

	// bone skeletons -> character mesh

	for (int boneIdx = 0; boneIdx < wowObject->Bones->Length; boneIdx++)
	{
		if (_skeletonNodesArray[boneIdx] == NULL)
			continue;

		auto skeletonNode = _skeletonNodesArray[boneIdx]->GetNode();

		// Задаем веса между костью и вершинами

		FbxCluster* cluster = FbxCluster::Create(_scene, "");
		cluster->SetLink(skeletonNode);
		cluster->SetLinkMode(FbxCluster::eTotalOne);

		auto boneInfluence = bonesInfluenceOnCharacterMeshVertices[boneIdx];

		if (boneInfluence != nullptr)
		{
			for (int i = 0; i < boneInfluence->Count; i++)
			{
				auto vertexIdx = (int)boneInfluence[i]->Item1;
				auto vertexWeight = (double)boneInfluence[i]->Item2;

				cluster->AddControlPointIndex(vertexIdx, vertexWeight);
			}
		}

		auto boneGlobalMatrix = skeletonNode->EvaluateGlobalTransform();
		cluster->SetTransformMatrix(characterMeshGlobalMatrix);
		cluster->SetTransformLinkMatrix(boneGlobalMatrix);

		characterSkin->AddCluster(cluster);
	}

	// Применяем skin (с записанными кластерами) к мешу
	characterMesh->AddDeformer(characterSkin);
}

void WowModelExporterFbx::Exporter::ApplySkinForAttachedObjectMesh(WowBone^ targetBone, FbxMesh* attachedObjectMesh)
{
	auto attachedObjectMeshGlobalMatrix = FbxAMatrix(attachedObjectMesh->GetNode()->EvaluateGlobalTransform());

	FbxSkin* attachedObjectSkin = FbxSkin::Create(_scene, "");

	// root skeleton -> attached object mesh

	auto rootSkeletonNode = _rootSkeleton->GetNode();

	FbxCluster* rootSkeletonCluster = FbxCluster::Create(_scene, "");
	rootSkeletonCluster->SetLink(rootSkeletonNode);
	rootSkeletonCluster->SetLinkMode(FbxCluster::eTotalOne);

	auto rootSkeletonNodeGlobalMatrix = rootSkeletonNode->EvaluateGlobalTransform();
	rootSkeletonCluster->SetTransformMatrix(attachedObjectMeshGlobalMatrix);
	rootSkeletonCluster->SetTransformLinkMatrix(rootSkeletonNodeGlobalMatrix);

	attachedObjectSkin->AddCluster(rootSkeletonCluster);

	// target bone skeleton -> attached object mesh

	auto skeletonNode = _skeletonNodesArray[targetBone->Index]->GetNode();

	// Задаем веса между костью и вершинами

	FbxCluster* cluster = FbxCluster::Create(_scene, "");
	cluster->SetLink(skeletonNode);
	cluster->SetLinkMode(FbxCluster::eTotalOne);

	auto vertexCount = attachedObjectMesh->GetControlPointsCount();
	for (int vertexIdx = 0; vertexIdx < vertexCount; vertexIdx++)
		cluster->AddControlPointIndex(vertexIdx, 1.0);

	auto boneGlobalMatrix = skeletonNode->EvaluateGlobalTransform();
	cluster->SetTransformMatrix(attachedObjectMeshGlobalMatrix);
	cluster->SetTransformLinkMatrix(boneGlobalMatrix);

	attachedObjectSkin->AddCluster(cluster);

	// Применяем skin (с записанными кластерами) к мешу
	attachedObjectMesh->AddDeformer(attachedObjectSkin);
}