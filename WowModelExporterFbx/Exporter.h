#pragma once

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Drawing;
using namespace WowModelExporterCore;

#include <fbxsdk.h>

namespace WowModelExporterFbx {
	public ref class Exporter
	{
	public:
		Exporter();
		~Exporter();
		Boolean ExportWowObject(WowObject^ wowObject, String^ exportDirectory);
	private:
		void CreateScene();
		void CreateObjects(WowObject^ wowObject);
		bool ExportSceneToFbxFile(String^ exportDirectory);

		// Создает материалы в порядке следования сабмешей
		// (то есть индекс wow сабмеша будет равен индексу fbx материала в ноде)
		void CreateMaterialsPerWowSubmesh(FbxNode* node, WowMeshWithMaterials^ wowMesh);

		FbxMesh* CreateMesh(FbxNode* node, WowMeshWithMaterials^ wowMeshWithMaterials);

		void CreateSkeleton(FbxNode* node, WowObject^ wowObject);
		FbxNode* CreateBoneForWowBoneAndItsChildren(FbxNode* parentNode, WowBone^ wowBone);
		FbxNode* CreateSkeletonElementsForWowBone(FbxNode* parentNode, WowBone^ wowBone);

		mutable FbxManager* _manager;
		mutable FbxScene* _scene;
		mutable List<Tuple<String^, Bitmap^>^>^ _textureFiles;
	};
}
