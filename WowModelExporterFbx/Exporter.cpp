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

int GetMaterialIndex(FbxNode* node, FbxSurfaceMaterial* material)
{
	for (int i = 0; i < node->GetMaterialCount(); i++)
	{
		if (material == node->GetMaterial(i))
			return i;
	}

	return -1;
}

WowModelExporterFbx::Exporter::Exporter()
{
	_manager = nullptr;
	_textureFiles = gcnew HashSet<TextureImage^>();
	_uniqueMaterials = gcnew Dictionary<String^, Material^>(10);
}

WowModelExporterFbx::Exporter::~Exporter()
{
	if (_manager != nullptr)
	{
		_manager->Destroy();
		_manager = nullptr;
	}
}

Boolean WowModelExporterFbx::Exporter::ExportWowObject(WowObject^ characterWowObject, List<BlendShapeUtility::BakedBlendshape^>^ bakedBlendshapes, String^ exportDirectory, Boolean useAscii)
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
	_bakedBlendshapes = bakedBlendshapes;

	CreateCharacter();

	auto exported = ExportSceneToFbxFile(exportDirectory, useAscii);

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
	_scene->GetRootNode()->AddChild(armatureNode);

	CreateAllSkinnedMeshesForCharacter(_scene->GetRootNode());

	delete[] _skeletonNodesArray;
}

bool WowModelExporterFbx::Exporter::ExportSceneToFbxFile(String^ exportDirectory, Boolean useAscii)
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

	exporter->SetFileExportVersion(FbxString(FBX_2014_00_COMPATIBLE), FbxSceneRenamer::eNone);

	int fileFormat = -1;

	int lFormatIndex, lFormatCount =
		_manager->GetIOPluginRegistry()->
		GetWriterFormatCount();
	for (lFormatIndex = 0; lFormatIndex < lFormatCount; lFormatIndex++)
	{
		if (_manager->GetIOPluginRegistry()->
			WriterIsFBX(lFormatIndex))
		{
			FbxString lDesc = _manager->GetIOPluginRegistry()->
				GetWriterFormatDescription(lFormatIndex);
			if (lDesc.Find(useAscii ? "FBX ascii" : "FBX binary") >= 0)
			{
				fileFormat = lFormatIndex;
				break;
			}
		}
	}

	auto exporterStatus = exporter->Initialize(fbxFileName, fileFormat, _manager->GetIOSettings());

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
	// создаем материалы, меши и применяем скин для основного объекта Body и дополнительных итемов которые привязываются к основным костям скелета

	for (int meshIdx = 0; meshIdx < _characterWowObject->Meshes->Count; meshIdx++)
	{
		// индекс - означает что это основной меш (Body)

		auto meshNodeName = meshIdx == 0 ? gcnew String("Body") : "Body_additional_" + Convert::ToString(meshIdx);

		auto meshNode = FbxNode::Create(_scene, GetString(meshNodeName));

		node->AddChild(meshNode);

		auto nodeMaterialIndexesPerSubmesh = CreateMaterialsForWowSubmeshes(meshNode, _characterWowObject->Meshes[meshIdx]);

		auto fbxMesh = CreateMesh(meshNode, _characterWowObject->Meshes[meshIdx], nodeMaterialIndexesPerSubmesh);

		ApplySkinForMesh(fbxMesh, _characterWowObject->Bones, _characterWowObject->Meshes[meshIdx]);

		if (meshIdx == 0)
			ApplyBlendShapes(fbxMesh);
	}

	// создаем материалы, меш и применяем скин для прикрепленных к костям предметов (они не привязываются к основному скелету а только к одной его кости и полностью всеми вершинами)

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

				auto nodeMaterialIndexesPerSubmesh = CreateMaterialsForWowSubmeshes(attachedObjectMeshNode, attachedWowObject->MainMesh);

				auto attachedObjectMesh = CreateMesh(attachedObjectMeshNode, attachedWowObject->MainMesh, nodeMaterialIndexesPerSubmesh);

				ApplySkinForMesh(attachedObjectMesh, _characterWowObject->Bones, attachedWowObject->MainMesh);
			}
		}
	}
}

array<Int32>^ WowModelExporterFbx::Exporter::CreateMaterialsForWowSubmeshes(FbxNode* node, WowMeshWithMaterials^ wowMesh)
{
	auto nodeMaterialIndexesPerSubmesh = gcnew array<Int32>(wowMesh->Submeshes->Count);

	for (int submeshIdx = 0; submeshIdx < wowMesh->Submeshes->Count; submeshIdx++)
	{
		auto fbxMaterial = GetOrCreateUniqueMaterial(wowMesh->Submeshes[submeshIdx]->Material);

		auto materialIdx = GetMaterialIndex(node, fbxMaterial);

		if (materialIdx == -1)
		{
			materialIdx = node->GetMaterialCount();
			node->AddMaterial(fbxMaterial);
		}

		nodeMaterialIndexesPerSubmesh[submeshIdx] = materialIdx;
	}
	
	return nodeMaterialIndexesPerSubmesh;
}

FbxSurfaceMaterial* WowModelExporterFbx::Exporter::GetOrCreateUniqueMaterial(WowMaterial^ wowMaterial)
{
	auto materialName = wowMaterial->GetUniqueName();

	if (_uniqueMaterials->ContainsKey(materialName))
		return _uniqueMaterials[materialName]->fbxMaterial;

	FbxSurfacePhong* material = FbxSurfacePhong::Create(_scene, GetString(materialName));

	auto gcMaterial = gcnew Material();
	gcMaterial->wowMaterial = wowMaterial;
	gcMaterial->fbxMaterial = material;

	_uniqueMaterials[materialName] = gcMaterial;

	material->ShadingModel.Set(FbxString("Phong"));

	// ToDo: у материала в блендере стоит галочка transparency. надо ее убрать. точнее сделать ее только там где материал действительно полупрозрачный

	if (wowMaterial->Type != WowMaterial::MaterialType::Transparent)
		material->TransparencyFactor.Set(0);
	else
		material->TransparencyFactor.Set(0.5);

	material->Diffuse.Set(FbxDouble3(1, 1, 1));
	material->DiffuseFactor.Set(1);

	material->Ambient.Set(FbxDouble3(1, 1, 1));
	material->AmbientFactor.Set(0);

	material->Emissive.Set(FbxDouble3(0, 0, 0));
	material->EmissiveFactor.Set(0);

	material->Specular.Set(FbxDouble3(1, 1, 1));
	material->SpecularFactor.Set(0);

	material->Shininess.Set(0);

	if (!_textureFiles->Contains(wowMaterial->MainImage))
		_textureFiles->Add(wowMaterial->MainImage);

	auto texture = FbxFileTexture::Create(_scene, GetString(wowMaterial->MainImage->Hash));

	texture->SetFileName(GetString(GetRelativeTextureFileName(wowMaterial->MainImage)));
	texture->SetTextureUse(FbxTexture::eStandard);
	texture->SetMappingType(FbxTexture::eUV);
	texture->SetMaterialUse(FbxFileTexture::eModelMaterial);
	texture->SetSwapUV(false);
	texture->SetTranslation(0.0, 0.0);
	texture->SetScale(1.0, 1.0);
	texture->SetRotation(0.0, 0.0);

	material->Diffuse.ConnectSrcObject(texture);

	return material;
}

FbxMesh* WowModelExporterFbx::Exporter::CreateMesh(FbxNode* node, WowMeshWithMaterials^ wowMesh, array<Int32>^ nodeMaterialIndexesPerSubmesh)
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



	// Маппинг материалов (индексы материалов (записанные в ноду node) прописываются при вызове mesh->BeginPolygon(индекс материала))

	FbxGeometryElementMaterial* materialElement = mesh->CreateElementMaterial();

	materialElement->SetMappingMode(FbxGeometryElement::eByPolygon);
	materialElement->SetReferenceMode(FbxGeometryElement::eIndexToDirect);

	// треугольники (на каждый сабмеш)

	for (int submeshIdx = 0; submeshIdx < wowMesh->Submeshes->Count; submeshIdx++)
	{
		auto wowSubmesh = wowMesh->Submeshes[submeshIdx];

		for (int triangleIdx = 0; triangleIdx < wowSubmesh->Triangles->Length; triangleIdx += 3)
		{
			mesh->BeginPolygon(nodeMaterialIndexesPerSubmesh[submeshIdx]);

			// (флипаем порядок отрисовки треугольников, т.к. флипали X позиции)
			mesh->AddPolygon(wowSubmesh->Triangles[triangleIdx + 2]);
			mesh->AddPolygon(wowSubmesh->Triangles[triangleIdx + 1]);
			mesh->AddPolygon(wowSubmesh->Triangles[triangleIdx]);

			mesh->EndPolygon();
		}
	}





	// smooth группы по edge-ам (не понятно как получить количество edge-ей - возвращается 0).
	// если применить, то нормали не учитываются, но работает расчет нормалей (из этих групп) для блендшейпов.
	// а просто нормали для блендшейпов юнити игнорит, и тупо берет базовые, что при сильных искажениях выливается в артефакты

	//FbxGeometryElementSmoothing* smoothingElement = mesh->CreateElementSmoothing();
	//smoothingElement->SetMappingMode(FbxGeometryElement::eByEdge);
	//smoothingElement->SetReferenceMode(FbxGeometryElement::eDirect);


	//int edgeCount = mesh->GetMeshEdgeCount();
	//edgeCount = 100500;

	//for (int i = 0; i < edgeCount; i++)
	//{
	//	smoothingElement->GetDirectArray().Add(1);
	//}

	return mesh;
}

array<List<Tuple<Int32, Single>^>^>^ WowModelExporterFbx::Exporter::CreateBonesInfluenceOnCharacterMeshVertices(array<WowBone^>^ wowBones, WowMeshWithMaterials^ wowMesh)
{
	auto influenceOnVertices = gcnew array<List<Tuple<Int32, Single>^>^>(wowBones->Length);

	auto vertices = wowMesh->Vertices;

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

void WowModelExporterFbx::Exporter::ApplySkinForMesh(FbxMesh* characterMesh, array<WowBone^>^ wowBones, WowMeshWithMaterials^ wowMesh)
{
	auto characterMeshGlobalMatrix = FbxAMatrix(characterMesh->GetNode()->EvaluateGlobalTransform());

	auto bonesInfluenceOnCharacterMeshVertices = CreateBonesInfluenceOnCharacterMeshVertices(wowBones, wowMesh);

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

	for (int boneIdx = 0; boneIdx < wowBones->Length; boneIdx++)
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

void WowModelExporterFbx::Exporter::ApplyBlendShapes(FbxMesh* mesh)
{
	FbxBlendShape* blendShape = FbxBlendShape::Create(_scene, "");

	for (auto bakedBlendshapeIdx = 0; bakedBlendshapeIdx < _bakedBlendshapes->Count; bakedBlendshapeIdx++)
	{
		auto bakedBlendshape = _bakedBlendshapes[bakedBlendshapeIdx];

		FbxBlendShapeChannel* blendShapeChannel = FbxBlendShapeChannel::Create(_scene, "");

		FbxShape* shape = FbxShape::Create(_scene, GetString(bakedBlendshape->BlendshapeName));

		// вершины

		shape->InitControlPoints(mesh->GetControlPointsCount());

		FbxVector4* shapeVertices = shape->GetControlPoints();
		FbxVector4* meshVertices = mesh->GetControlPoints();

		// Копируем все позиции вершин
		for (int i = 0; i < mesh->GetControlPointsCount(); i++)
			shapeVertices[i] = meshVertices[i];

		for each (auto bakedBlendshapeChange in bakedBlendshape->Changes)
			shapeVertices[bakedBlendshapeChange.Key] = ConvertPositionFromWowVec3(bakedBlendshapeChange.Value->Position.X, bakedBlendshapeChange.Value->Position.Y, bakedBlendshapeChange.Value->Position.Z);

		// нормали
		// юнити их все равно игнорит собака. блендер тоже игнорит, но он хотя бы перестраивает их сам на основе базовых нормалей из меша. юнити тупо оставляет базовые нормали и не парится.
		// задал тут вопрос https://answers.unity.com/questions/1579110/blendshape-normals-ignored.html

		FbxGeometryElementNormal* meshNormalElement = mesh->GetElementNormal();

		FbxGeometryElementNormal* shapeNormalElement = shape->CreateElementNormal();
		shapeNormalElement->SetMappingMode(FbxGeometryElement::eByControlPoint);
		shapeNormalElement->SetReferenceMode(FbxGeometryElement::eDirect);

		// Копируем все нормали
		for (int i = 0; i < mesh->GetControlPointsCount(); i++)
			shapeNormalElement->GetDirectArray().Add(meshNormalElement->GetDirectArray().GetAt(i));

		for each (auto bakedBlendshapeChange in bakedBlendshape->Changes)
		{
			shapeNormalElement->GetDirectArray().SetAt(bakedBlendshapeChange.Key, ConvertPositionFromWowVec3(bakedBlendshapeChange.Value->Normal.X, bakedBlendshapeChange.Value->Normal.Y, bakedBlendshapeChange.Value->Normal.Z, 0));
		}

		blendShapeChannel->AddTargetShape(shape);

		blendShape->AddBlendShapeChannel(blendShapeChannel);
	}

	mesh->AddDeformer(blendShape);
}