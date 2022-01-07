using System;
using System.Runtime.InteropServices;

struct CA210DataStruct
{
    public float CA210fX;
    public float CA210fY;
    public float CA210fZ;
    public float CA210fFlckrJEITA;
    public long CA210lNumber;
    public float CA210fLv;
    public float CA210fUd;
    public float CA210fVd;
    public long CA210lT;
    public float CA210fDEUser;
    public float CA210fFlckrFMA;
    public float CA210fSy;
    public float CA210fSx;
    public float CA210fDuv;
    public float CA210fUsUser;
    public float CA210fVsUser;
    public float CA210fLsUser;
    public float CA210fLvfL;
    public float CA210fR;
    public float CA210fG;
    public float CA210fB;
    public long CA210lRd;
    public long CA210lRad;
    public long CA210lRfma;
    public long CA210lRjeita;
}

namespace WindowsFormsApp_BOE_Tool
{
    internal static class CA
    {
        [DllImport("CA210Ctrl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ca210Connect(long nChannelNO);

        [DllImport("CA210Ctrl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ca210SetSyncMode(int nSyncMode);

        [DllImport("CA210Ctrl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ca210SetSpeed(int nSpeed);

        [DllImport("CA210Ctrl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ca210ZeroCal();

        [DllImport("CA210Ctrl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern CA210DataStruct ca210Measure();
    }
}