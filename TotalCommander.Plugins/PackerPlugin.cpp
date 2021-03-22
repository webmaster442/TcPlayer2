/*

*/
#include <vector>
#include <fstream>
#include <Windows.h>
#include <cwchar>
#include <string>
#include <Shlwapi.h>
#include "wcxhead.h"

using namespace std;

HINSTANCE hinst;
tChangeVolProc PackerChangeVolProc;
tProcessDataProc PackerProcessDataProc;

EXTERN_C IMAGE_DOS_HEADER __ImageBase;

#define PROGRAMNAME L"TCPlayer.exe"

BOOL APIENTRY DllMain(HANDLE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		hinst = (HINSTANCE)hModule;
		break;
	case DLL_PROCESS_DETACH:
		break;
	case DLL_THREAD_ATTACH:
		break;
	case DLL_THREAD_DETACH:
		break;
	}
	return TRUE;
}

/*=============================================================================
Private internal functions
=============================================================================*/
static WCHAR* charToWChar(const char* text)
{
	const size_t size = strlen(text) + 1;
	wchar_t* wText = new wchar_t[size];
	mbstowcs(wText, text, size);
	return wText;
}

/*=============================================================================
Unpacking stuff, mandatory for plugin load
=============================================================================*/
#ifdef __cplusplus
extern "C" {
#endif
__declspec(dllexport) HANDLE __stdcall OpenArchive(tOpenArchiveData *ArchiveData)
{
	ArchiveData->OpenResult = E_UNKNOWN_FORMAT;
	return 0;
}


__declspec(dllexport) int __stdcall ReadHeader(HANDLE hArcData, tHeaderData *HeaderData)
{
	return E_BAD_ARCHIVE;
}

__declspec(dllexport) int __stdcall ProcessFile(HANDLE hArcData, int Operation, char *DestPath, char *DestName)
{
	return E_BAD_ARCHIVE;
}

__declspec(dllexport) int __stdcall CloseArchive(HANDLE hArcData)
{
	return 0;
}

__declspec(dllexport) void __stdcall SetChangeVolProc(HANDLE hArcData, tChangeVolProc pChangeVolProc1)
{
	PackerChangeVolProc = pChangeVolProc1;
}

__declspec(dllexport) void __stdcall SetProcessDataProc(HANDLE hArcData, tProcessDataProc pProcessDataProc)
{
	PackerProcessDataProc = pProcessDataProc;
}

/*=============================================================================
Packing stuff
=============================================================================*/
__declspec(dllexport) int __stdcall GetPackerCaps()
{
	return PK_CAPS_NEW | PK_CAPS_MULTIPLE | PK_CAPS_OPTIONS | PK_CAPS_HIDE;
}

__declspec(dllexport) void __stdcall ConfigurePacker(HWND Parent, HINSTANCE DllInstance)
{
	MessageBox(Parent, L"This plugin allows you to send multiple files to the TC Player Playlist",
		L"TC Player Playlist packer", MB_ICONINFORMATION);
}

__declspec(dllexport) int __stdcall PackFilesW(WCHAR *PackedFile, WCHAR *SubPath, WCHAR *SrcPath, WCHAR *AddList, int Flags)
{
	vector<wstring> vector;
	WCHAR temp_file[MAX_PATH];
	GetTempPathW(MAX_PATH, temp_file);
	PathCombineW(temp_file, temp_file, L"tcplayer_pack.m3u");
	WCHAR *p;
	for (p = AddList; *p != '\0'; p += wcslen(p) + 1)
	{
		vector.push_back(p);
	}

	wofstream tempfile;
	tempfile.open(temp_file, wfstream::out | wfstream::trunc);
	if (tempfile.good())
	{
		WCHAR line[MAX_PATH];
		for (wstring file : vector)
		{
			PathCombineW(line, SrcPath, file.c_str());
			tempfile << line << endl;
		}
		tempfile.close();

		WCHAR dllpath[MAX_PATH];

		GetModuleFileName((HINSTANCE)&__ImageBase, dllpath, MAX_PATH);
		PathRemoveFileSpec(dllpath);

		WCHAR safefile[MAX_PATH];
		swprintf(safefile, L"\"%s\"", temp_file);

		SHELLEXECUTEINFO ShExecInfo = { 0 };
		ShExecInfo.cbSize = sizeof(SHELLEXECUTEINFO);
		ShExecInfo.fMask = SEE_MASK_NOCLOSEPROCESS;
		ShExecInfo.hwnd = NULL;
		ShExecInfo.lpVerb = NULL;
		ShExecInfo.lpFile = PROGRAMNAME;
		ShExecInfo.lpParameters = safefile;
		ShExecInfo.lpDirectory = dllpath;
		ShExecInfo.nShow = SW_NORMAL;
		ShExecInfo.hInstApp = NULL;
		ShellExecuteEx(&ShExecInfo);
		Sleep(25);
		return 0;
	}
	else
	{
		return E_ECREATE;
	}
}

__declspec(dllexport) int __stdcall PackFiles(char *PackedFile, char *SubPath, char *SrcPath, char *AddList, int Flags)
{
	return PackFilesW(charToWChar(PackedFile),
					  charToWChar(SubPath),
					  charToWChar(SrcPath),
					  charToWChar(AddList),
					  Flags);
}

#ifdef __cplusplus
}; // end of extern "C"
#endif