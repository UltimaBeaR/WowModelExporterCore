#pragma once

using namespace System;
using namespace WowModelExporterCore;

#include <fbxsdk.h>

namespace WowModelExporterFbx {
	public ref class Exporter
	{
	public:
		Exporter();
		Boolean ExportWowObject(WowObject^ wowObject, String^ fbxFilename);
	private:
		FbxScene* CreateScene();

		void CreateObjects(FbxScene* scene, WowObject^ wowObject);

		// Создает материалы в порядке следования сабмешей
		// (то есть индекс wow сабмеша будет равен индексу fbx материала в ноде)
		void CreateMaterialsPerWowSubmesh(FbxNode* node, WowMeshWithMaterials^ wowMesh);

		FbxMesh* CreateMesh(FbxNode* node, WowMeshWithMaterials^ wowMeshWithMaterials);
		bool ExportSceneToFbxFile(FbxScene* scene, const char* fbxFilename);

		mutable FbxManager* _manager;
	};
}
