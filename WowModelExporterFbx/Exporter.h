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

		array<List<Tuple<Int32, Single>^>^>^ CreateBonesInfluenceOnVertices(WowObject^ wowObject);

		// Создает материалы в порядке следования сабмешей
		// (то есть индекс wow сабмеша будет равен индексу fbx материала в ноде)
		void CreateMaterialsPerWowSubmesh(FbxNode* node, WowMeshWithMaterials^ wowMesh);

		FbxMesh* CreateMesh(FbxNode* node, WowMeshWithMaterials^ wowMeshWithMaterials);

		void CreateSkeleton(FbxNode* node, WowObject^ wowObject);
		FbxNode* CreateBoneForWowBoneAndItsChildren(FbxNode* parentNode, WowBone^ wowBone);
		FbxNode* CreateSkeletonElementsForWowBone(FbxNode* parentNode, WowBone^ wowBone);

		mutable FbxManager* _manager;
		mutable FbxScene* _scene;

		// Tuple<относительный путь файла текстуры, сама текстура>
		mutable List<Tuple<String^, Bitmap^>^>^ _textureFiles;

		// Пересоздается при работе с каждым WowObject:

		// Это массив, параллельный исходному массиву костей (индексы такие же), в кажом элементе - список с Tuple<индексами вершин, вес для кости на эту вершину>.
		// То есть это по сути массив костей с инфой о том на какие вершины каждая кость влияет и на сколько.
		// Лист в массиве может быть null
		mutable array<List<Tuple<Int32, Single>^>^>^ _currentWowObjectBonesInfluenceOnVertices;

		mutable FbxSkin* _currentWowObjectSkin;

		mutable FbxMesh* _currentWowObjectMesh;
		mutable FbxAMatrix* _currentWowObjectMeshObjectGlobalMatrix;
	};
}
