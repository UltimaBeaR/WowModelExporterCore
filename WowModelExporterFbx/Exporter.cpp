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

	auto scene = CreateScene(wowObject);

	auto exported = ExportSceneToFbxFile(scene, pFbxFilename);

	Marshal::FreeHGlobal((System::IntPtr)(void*)pFbxFilename);

	_manager->Destroy();
	_manager = nullptr;

	return Boolean(exported);
}

FbxScene* WowModelExporterFbx::Exporter::CreateScene(WowObject ^ wowObject)
{
	auto scene = FbxScene::Create(_manager, "test_scene");

	auto rootNode = scene->GetRootNode();

	auto childNode = FbxNode::Create(scene, "test_node");

	rootNode->AddChild(childNode);

	return scene;
}

bool WowModelExporterFbx::Exporter::ExportSceneToFbxFile(FbxScene* scene, const char* fbxFilename)
{
	auto exporter = FbxExporter::Create(_manager, "");
	auto exporterStatus = exporter->Initialize(fbxFilename, -1, _manager->GetIOSettings());

	return exporter->Export(scene);
}
