#include "stdafx.h"

#include "WowModelExporterFbx.h"

#include <fbxsdk.h>

String ^ WowModelExporterFbx::Class1::GetStuff()
{
	FbxManager* lSdkManager = FbxManager::Create();

	return gcnew String(lSdkManager->GetVersion());
}
