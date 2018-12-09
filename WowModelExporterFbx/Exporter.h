#pragma once

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Drawing;
using namespace WowheadModelLoader;
using namespace WowModelExporterCore;

#include <fbxsdk.h>

// ToDo: в новой юнити (2018) материалы для attached объектов импортируются криво.
// в старой 5ой юнити все норм, там материалы создаются в отдельной папке на основе того что лежит в fbx, в новой же материалы там вытаскиваются
// каким то раком прямо из fbx и они вот такие вот кривые получаются. надо разобраться, чтобы везде работало, к тому же vrchat вроде собираются перевести на новую юнити

namespace WowModelExporterFbx {
	public ref class Exporter
	{
	public:
		Exporter();
		~Exporter();
		Boolean ExportWowObject(WowObject^ wowObject, Dictionary<String^, Dictionary<Int32, BlendShapeBaker::Vertex^>^>^ bakedBlendshapes, String^ exportDirectory, Boolean useAscii);
	private:
		void CreateScene();
		void CreateCharacter();
		bool ExportSceneToFbxFile(String^ exportDirectory, Boolean useAscii);

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
