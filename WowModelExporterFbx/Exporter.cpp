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

Tuple<String^, Bitmap^>^ FindExistingTextureFile(List<Tuple<String^, Bitmap^>^>^ textureFiles, Bitmap^ texture)
{
	for (int i = 0; i < textureFiles->Count; i++)
	{
		if (textureFiles[i]->Item2 == texture)
			return textureFiles[i];
	}

	return nullptr;
}

WowModelExporterFbx::Exporter::Exporter()
{
	_manager = nullptr;
	_textureFiles = gcnew List<Tuple<String^, Bitmap^>^>(0);
}

WowModelExporterFbx::Exporter::~Exporter()
{
	if (_manager != nullptr)
	{
		_manager->Destroy();
		_manager = nullptr;
	}
}

Boolean WowModelExporterFbx::Exporter::ExportWowObject(WowObject^ wowObject, String^ exportDirectory)
{
	if (wowObject == nullptr)
		throw gcnew ArgumentException("wowObject");

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
	CreateObjects(wowObject);

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

void WowModelExporterFbx::Exporter::CreateObjects(WowObject^ wowObject)
{
	auto armatureNode = FbxNode::Create(_scene, "Armature");
	auto bodyNode = FbxNode::Create(_scene, "Body");

	armatureNode->AddChild(bodyNode);

	CreateSkeleton(armatureNode, wowObject);

	CreateMaterialsPerWowSubmesh(bodyNode, wowObject->Mesh);
	CreateMesh(bodyNode, wowObject->Mesh);

	_scene->GetRootNode()->AddChild(armatureNode);
}

bool WowModelExporterFbx::Exporter::ExportSceneToFbxFile(String^ exportDirectory)
{
	for (int i = 0; i < _textureFiles->Count; i++)
	{
		auto relativeFileName = _textureFiles[i]->Item1;
		auto bitmap = _textureFiles[i]->Item2;

		auto absoluteFileName = Path::Combine(exportDirectory, relativeFileName);

		(gcnew FileInfo(absoluteFileName))->Directory->Create();

		bitmap->Save(absoluteFileName, ImageFormat::Png);
	}

	auto fbxFileName = GetString(Path::Combine(exportDirectory, "character.fbx"));

	auto exporter = FbxExporter::Create(_manager, "");
	auto exporterStatus = exporter->Initialize(fbxFileName, -1, _manager->GetIOSettings());

	return exporter->Export(_scene);
}

void WowModelExporterFbx::Exporter::CreateMaterialsPerWowSubmesh(FbxNode* node, WowMeshWithMaterials^ wowMesh)
{
	for (int submeshIdx = 0; submeshIdx < wowMesh->Submeshes->Count; submeshIdx++)
	{
		auto wowMaterial = wowMesh->Submeshes[submeshIdx]->Material;

		auto materialName = FbxString("material submesh[") + submeshIdx + "]";
		auto textureName = FbxString("main texture submesh[") + submeshIdx + "]";

		FbxString textureRelativeFileName;;
		
		auto mainTextureFile = FindExistingTextureFile(_textureFiles, wowMaterial->MainImage);
		if (mainTextureFile == nullptr)
		{
			textureRelativeFileName = FbxString("Textures/main_tex_") + submeshIdx + ".png";

			_textureFiles->Add(gcnew Tuple<String^, Bitmap^>(
				gcnew String(textureRelativeFileName.Buffer()),
				wowMaterial->MainImage));
		}
		else
			textureRelativeFileName = FbxString(GetString(mainTextureFile->Item1));

		FbxSurfacePhong* material = FbxSurfacePhong::Create(_scene, materialName.Buffer());

		material->ShadingModel.Set(FbxString("Phong"));

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

		texture->SetFileName(textureRelativeFileName.Buffer());
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
		vertices[i] = FbxVector4(wowVertex->Position.X, wowVertex->Position.Y, wowVertex->Position.Z);
	}

	// нормали

	FbxGeometryElementNormal* normalElement = mesh->CreateElementNormal();
	normalElement->SetMappingMode(FbxGeometryElement::eByControlPoint);
	normalElement->SetReferenceMode(FbxGeometryElement::eDirect);

	for (int i = 0; i < wowMesh->Vertices->Length; i++)
	{
		auto wowVertex = wowMesh->Vertices[i];
		normalElement->GetDirectArray().Add(FbxVector4(wowVertex->Normal.X, wowVertex->Normal.Y, wowVertex->Normal.Z, 0));
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

			mesh->AddPolygon(wowSubmesh->Triangles[triangleIdx]);
			mesh->AddPolygon(wowSubmesh->Triangles[triangleIdx + 1]);
			mesh->AddPolygon(wowSubmesh->Triangles[triangleIdx + 2]);

			mesh->EndPolygon();
		}
	}

	return mesh;
}

void WowModelExporterFbx::Exporter::CreateSkeleton(FbxNode* node, WowObject^ wowObject)
{
	FbxSkeleton* skeletonRoot = FbxSkeleton::Create(_scene, "skeleton_root");
	skeletonRoot->SetSkeletonType(FbxSkeleton::eRoot);
	node->SetNodeAttribute(skeletonRoot);

	auto wowRootBone = wowObject->GetRootBone();

	if (wowRootBone == nullptr)
		return;

	CreateBoneForWowBoneAndItsChildren(node, wowRootBone);
}

FbxNode* WowModelExporterFbx::Exporter::CreateBoneForWowBoneAndItsChildren(FbxNode* parentNode, WowBone^ wowBone)
{
	auto boneNode = CreateSkeletonElementsForWowBone(parentNode, wowBone);

	for (int i = 0; i < wowBone->ChildBones->Count; i++)
		CreateBoneForWowBoneAndItsChildren(boneNode, wowBone->ChildBones[i]);

	return boneNode;
}

FbxNode* WowModelExporterFbx::Exporter::CreateSkeletonElementsForWowBone(FbxNode* parentNode, WowBone^ wowBone)
{
	auto wowBoneName = wowBone->GetName();

	auto boneName = wowBoneName != nullptr ? GetString(wowBoneName) : (FbxString("bone ") + (int)wowBone->Id).Buffer();

	auto boneNode = FbxNode::Create(_scene, boneName);

	FbxSkeleton* skeleton = FbxSkeleton::Create(_scene, boneName);
	skeleton->SetSkeletonType(FbxSkeleton::eLimbNode);
	skeleton->Size.Set(1.0);
	boneNode->SetNodeAttribute(skeleton);

	// ToDo: надо чтоб позиция была локалной задана изначально
	boneNode->LclTranslation.Set(FbxVector4(wowBone->LocalPosition.X, wowBone->LocalPosition.Y, wowBone->LocalPosition.Z));

	parentNode->AddChild(boneNode);

	return boneNode;
}