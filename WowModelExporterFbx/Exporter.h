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
		FbxScene* CreateScene(WowObject^ wowObject);
		bool ExportSceneToFbxFile(FbxScene* scene, const char* fbxFilename);

		mutable FbxManager* _manager;
	};
}
