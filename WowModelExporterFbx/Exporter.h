#pragma once

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Drawing;
using namespace WowheadModelLoader;
using namespace WowModelExporterCore;

#include <fbxsdk.h>

namespace WowModelExporterFbx {
	public ref struct Material {
	public:
		WowMaterial^ wowMaterial;
		FbxSurfaceMaterial* fbxMaterial;
	};

	public ref class Exporter
	{
	public:
		Exporter();
		~Exporter();
		Boolean ExportWowObject(WowObject^ wowObject, Dictionary<String^, Dictionary<Int32, BlendShapeUtility::Vertex^>^>^ bakedBlendshapes, String^ exportDirectory, Boolean useAscii);
	private:
		void CreateScene();
		void CreateCharacter();
		bool ExportSceneToFbxFile(String^ exportDirectory, Boolean useAscii);

		void CreateCharacterSkeleton(FbxNode* node);
		FbxNode* CreateChildBones(FbxNode* parentNode, WowBone^ wowBone);
		FbxNode* CreateChildBone(FbxNode* parentNode, WowBone^ wowBone);

		void CreateAllSkinnedMeshesForCharacter(FbxNode* node);

		array<Int32>^ CreateMaterialsForWowSubmeshes(FbxNode* node, WowMeshWithMaterials^ wowMesh);
		FbxSurfaceMaterial* GetOrCreateUniqueMaterial(WowMaterial^ wowMaterial);

		FbxMesh* CreateMesh(FbxNode* node, WowMeshWithMaterials^ wowMeshWithMaterials, array<Int32>^ nodeMaterialIndexesPerSubmesh);

		array<List<Tuple<Int32, Single>^>^>^ CreateBonesInfluenceOnCharacterMeshVertices(WowObject^ wowObject);
		void ApplySkinForCharacterMesh(WowObject^ wowObject, FbxMesh* characterMesh);
		void ApplySkinForAttachedObjectMesh(WowBone^ targetBone, FbxMesh* attachedObjectMesh);

		void ApplyBlendShapes(FbxMesh* mesh);

		mutable FbxManager* _manager;
		mutable FbxScene* _scene;

		mutable HashSet<TextureImage^>^ _textureFiles;
		mutable Dictionary<String^, Material^>^ _uniqueMaterials;

		mutable Dictionary<String^, Dictionary<Int32, BlendShapeUtility::Vertex^>^>^ _bakedBlendshapes;
		mutable WowObject^ _characterWowObject;

		// Массив созданных Fbx Костей по индексам костей из WowBone::Bones
		mutable FbxSkeleton** _skeletonNodesArray;
		mutable FbxSkeleton* _rootSkeleton;

		mutable array<List<Tuple<Int32, Single>^>^>^ _bodyBonesInfluenceOnVertices;
	};
}
