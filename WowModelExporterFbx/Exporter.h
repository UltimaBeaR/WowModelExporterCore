#pragma once

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Drawing;
using namespace WowheadModelLoader;
using namespace WowModelExporterCore;

#include <fbxsdk.h>

namespace WowModelExporterFbx {
	public ref class Exporter
	{
	public:
		Exporter();
		~Exporter();
		Boolean ExportWowObject(WowObject^ wowObject, Dictionary<String^, Dictionary<Int32, BlendShapeBaker::Vertex^>^>^ bakedBlendshapes, String^ exportDirectory);
	private:
		void CreateScene();
		void CreateCharacter();
		bool ExportSceneToFbxFile(String^ exportDirectory);

		void CreateCharacterSkeleton(FbxNode* node);
		FbxNode* CreateChildBones(FbxNode* parentNode, WowBone^ wowBone);
		FbxNode* CreateChildBone(FbxNode* parentNode, WowBone^ wowBone);

		void CreateAllSkinnedMeshesForCharacter(FbxNode* node);

		// Создает материалы в порядке следования сабмешей
		// (то есть индекс wow сабмеша будет равен индексу fbx материала в ноде)
		void CreateMaterialsPerWowSubmesh(FbxNode* node, WowMeshWithMaterials^ wowMesh, char* fileNamePrefix);

		FbxMesh* CreateMesh(FbxNode* node, WowMeshWithMaterials^ wowMeshWithMaterials);

		array<List<Tuple<Int32, Single>^>^>^ CreateBonesInfluenceOnCharacterMeshVertices(WowObject^ wowObject);
		void ApplySkinForCharacterMesh(WowObject^ wowObject, FbxMesh* characterMesh);
		void ApplySkinForAttachedObjectMesh(WowBone^ targetBone, FbxMesh* attachedObjectMesh);

		void ApplyBlendShapes(FbxMesh* mesh);

		mutable FbxManager* _manager;
		mutable FbxScene* _scene;

		mutable HashSet<TextureImage^>^ _textureFiles;

		mutable Dictionary<String^, Dictionary<Int32, BlendShapeBaker::Vertex^>^>^ _bakedBlendshapes;
		mutable WowObject^ _characterWowObject;

		// Массив созданных Fbx Костей по индексам костей из WowBone::Bones
		mutable FbxSkeleton** _skeletonNodesArray;
		mutable FbxSkeleton* _rootSkeleton;

		mutable array<List<Tuple<Int32, Single>^>^>^ _bodyBonesInfluenceOnVertices;
	};
}
