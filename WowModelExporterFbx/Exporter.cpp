#include "stdafx.h"

#include <fbxsdk.h>
#include <fbxsdk\fileio\fbxiosettings.h>

#include "Exporter.h"

using namespace System::Runtime::InteropServices;

WowModelExporterFbx::Exporter::Exporter()
{
	_manager = nullptr;
}

Boolean WowModelExporterFbx::Exporter::ExportWowObject(WowObject^ wowObject, String^ fbxFilename)
{
	_manager = FbxManager::Create();

	auto ioSettings = FbxIOSettings::Create(_manager, IOSROOT);
	_manager->SetIOSettings(ioSettings);

	auto pFbxFilename = (char*)(void*)Marshal::StringToHGlobalAnsi(fbxFilename);

	auto scene = CreateScene();
	CreateObjects(scene, wowObject);

	auto exported = ExportSceneToFbxFile(scene, pFbxFilename);

	Marshal::FreeHGlobal((System::IntPtr)(void*)pFbxFilename);

	_manager->Destroy();
	_manager = nullptr;

	return Boolean(exported);
}

FbxScene* WowModelExporterFbx::Exporter::CreateScene()
{
	auto scene = FbxScene::Create(_manager, "test_scene");

	FbxSystemUnit::m.ConvertScene(scene);
	FbxAxisSystem::MayaYUp.ConvertScene(scene);

	return scene;
}

void WowModelExporterFbx::Exporter::CreateObjects(FbxScene* scene, WowObject^ wowObject)
{
	auto rootNode = scene->GetRootNode();

	auto childNode = FbxNode::Create(scene, "test_node");

	CreateMaterialsPerWowSubmesh(childNode, wowObject->Mesh);
	CreateMesh(childNode, wowObject->Mesh);

	rootNode->AddChild(childNode);
}

void WowModelExporterFbx::Exporter::CreateMaterialsPerWowSubmesh(FbxNode* node, WowMeshWithMaterials^ wowMesh)
{
	for (int i = 0; i < wowMesh->Submeshes->Count; i++)
	{
		auto wowMaterial = wowMesh->Submeshes[i]->Material;

		auto materialName = FbxString("material submesh[") + i + "]";

		FbxSurfacePhong* material = FbxSurfacePhong::Create(_manager, materialName.Buffer());

		material->Emissive.Set(FbxDouble3(0.5, 0.5, 0.5));
		material->Ambient.Set(FbxDouble3(0.5, 0.5, 0.5));
		material->Diffuse.Set(FbxDouble3(0.5, 0.5, 0.5));

		material->TransparencyFactor.Set(0.0);
		material->ShadingModel.Set(FbxString("Phong"));
		material->Shininess.Set(0.5);

		node->AddMaterial(material);
	}
}

FbxMesh* WowModelExporterFbx::Exporter::CreateMesh(FbxNode* node, WowMeshWithMaterials^ wowMesh)
{
	auto mesh = FbxMesh::Create(_manager, "test_mesh");
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

	FbxGeometryElementNormal* lNormalElement = mesh->CreateElementNormal();
	lNormalElement->SetMappingMode(FbxGeometryElement::eByControlPoint);
	lNormalElement->SetReferenceMode(FbxGeometryElement::eDirect);

	for (int i = 0; i < wowMesh->Vertices->Length; i++)
	{
		auto wowVertex = wowMesh->Vertices[i];
		lNormalElement->GetDirectArray().Add(FbxVector4(wowVertex->Normal.X, wowVertex->Normal.Y, wowVertex->Normal.Z, 0));
	}

	// треугольники (на каждый сабмеш)

	FbxGeometryElementMaterial* lMaterialElement = mesh->CreateElementMaterial();
	lMaterialElement->SetMappingMode(FbxGeometryElement::eByPolygon);
	lMaterialElement->SetReferenceMode(FbxGeometryElement::eIndexToDirect);

	lMaterialElement->GetIndexArray().Add(0);

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

bool WowModelExporterFbx::Exporter::ExportSceneToFbxFile(FbxScene* scene, const char* fbxFilename)
{
	auto exporter = FbxExporter::Create(_manager, "");
	auto exporterStatus = exporter->Initialize(fbxFilename, -1, _manager->GetIOSettings());

	return exporter->Export(scene);
}