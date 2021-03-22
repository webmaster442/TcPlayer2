/*

*/
#include <Windows.h>
#include <cwchar>
#include <Shlwapi.h>
#include "listplug.h"

HINSTANCE hinst;
HWND timer = 0;
EXTERN_C IMAGE_DOS_HEADER __ImageBase;

#define EXTENSIONS "MULTIMEDIA & (EXT=\"MP3\" | EXT=\"MP3PRO\" | EXT=\"MP1\" | EXT=\"MP2\" | EXT=\"M4A\" | EXT=\"M4B\" | EXT=\"AAC\" | EXT=\"FLAC\" | EXT=\"AC3\" | EXT=\"WV\" | EXT=\"WAV\" | EXT=\"AIFF\" | EXT=\"AIF\" | EXT=\"WMA\" | EXT=\"MIDI\" | EXT=\"MID\" | EXT=\"RMI\" | EXT=\"KAR\" | EXT=\"OGG\" | EXT=\"MOD\" | EXT=\"XM\" | EXT=\"IT\" | EXT=\"S3M\" | EXT=\"MTM\" | EXT=\"UMX\" | EXT=\"MO3\" | EXT=\"M3U\" | EXT=\"PLS\" | EXT=\"WPL\" | EXT=\"APE\" | EXT=\"MPC\" | EXT=\"MP+\" | EXT=\"MPP\" | EXT=\"OFR\" | EXT=\"OFS\" | EXT=\"SPX\" | EXT=\"TTA\" | EXT=\"DSF\" | EXT=\"DSDIFF\" | EXT=\"OPUS\")"
#define PROGRAMNAME L"TCPlayer.exe"

#ifndef countof
#define countof(str) (sizeof(str)/sizeof(str[0]))
#endif // countof

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
char* strlcpy(char* p, char*p2, int maxlen)
{
	if ((int)strlen(p2) >= maxlen)
	{
		strncpy(p, p2, maxlen);
		p[maxlen] = 0;
	}
	else strcpy(p, p2);
	return p;
}

WCHAR* awlcopy(WCHAR* outname, char* inname, int maxlen)
{
	if (inname) {
		MultiByteToWideChar(CP_ACP, 0, inname, -1, outname, maxlen);
		outname[maxlen] = 0;
		return outname;
	}
	else
		return NULL;
}

void CALLBACK timer_code(HWND hwnd, UINT uMsg, UINT timerId, DWORD dwTime)
{
	KillTimer(timer, 0);
	DestroyWindow(timer);
}

HWND CallKiller(HWND aListerWindow)
{
	HWND handle =  CreateWindowEx(WS_EX_CONTROLPARENT,
								  L"TCPlayer_LISTERWIN",
								  L"TCPlayer_LISTERWIN",
								  WS_CHILD | WS_VISIBLE | WS_CLIPSIBLINGS,
								  0, 0, 10, 10, aListerWindow, 0, hinst, NULL);
	timer = aListerWindow;
	SetTimer(timer, 0, 250, (TIMERPROC)&timer_code);
	return handle;
}

/*=============================================================================
Lister stuff
=============================================================================*/
#ifdef __cplusplus
extern "C" {
#endif


__declspec(dllexport) HWND __stdcall ListLoad(HWND ParentWin, char* FileToLoad, int ShowFlags)
{
	WCHAR FileToLoadW[MAX_PATH];
	return ListLoadW(ParentWin, awlcopy(FileToLoadW, FileToLoad, countof(FileToLoadW) - 1), ShowFlags);
}

__declspec(dllexport) HWND __stdcall ListLoadW(HWND ParentWin, WCHAR* FileToLoad, int ShowFlags)
{
	WCHAR safefile[MAX_PATH];
	WCHAR dllpath[MAX_PATH];

	GetModuleFileName((HINSTANCE)&__ImageBase, dllpath, MAX_PATH);
	PathRemoveFileSpec(dllpath);

	swprintf(safefile, L"\"%s\"", FileToLoad);

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

	SetWindowPos(ParentWin, 0, -100, -100, 0, 0, SWP_HIDEWINDOW);
	return CallKiller(ParentWin);
}

__declspec(dllexport) void __stdcall ListGetDetectString(char* DetectString, int maxlen)
{
	strlcpy(DetectString, (char*)EXTENSIONS, maxlen);
}

__declspec(dllexport) void __stdcall ListCloseWindow(HWND ListWin)
{
	DestroyWindow(ListWin);
	return;
}

#ifdef __cplusplus
}; // end of extern "C"
#endif
