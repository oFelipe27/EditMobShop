using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MOBEditor
{
    public class Structs
    {
        public const int MAX_EQUIP = 18;
        public const int MAX_CARRY = 64;
        public const int ITEMNAME_LENGTH = 64;
        public const int MAX_STATICEFFECT = 12;
        public const int MAX_ITEMLIST = 6500;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct STRUCT_ITEM_AFFECT2
        {
            public byte cEffect;
            public byte cValue;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct STRUCT_ITEM
        {
            public Int16 sIndex;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public STRUCT_ITEM_AFFECT2[] stEffect;//8

            public void Clear()
            {
                this.sIndex = 0;

                for (Int32 i = 0; i < 3; ++i)
                {
                    this.stEffect[i].cEffect = 0;
                    this.stEffect[i].cValue = 0;
                }               
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct STRUCT_AFFECT
        {
            public byte Type;
            public byte Value;
            public UInt16 Level;
            public UInt32 Time;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct STRUCT_SCORE
        {
            public Int32 Level;
            public Int32 Ac;
            public Int32 Damage;
            public byte Merchant;
            public byte AttackRun;
            public byte Direction;
            public byte ChaosRate;
            public Int32 MaxHp;
            public Int32 MaxMp;
            public Int32 Hp;
            public Int32 Mp;

            public Int16 Str;
            public Int16 Int;
            public Int16 Dex;
            public Int16 Con;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public Int16[] Special;
        };

        [StructLayout(LayoutKind.Sequential/*, Pack = 1*/)] // SEM ALINHAMENTO
        public struct STRUCT_MOB
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string MobName;

            public byte Clan;
            public byte Merchant;
            public UInt16 Guild;
            public byte Class;
            public UInt16 Rsv;
            public byte Quest;
            public Int32 Coin;
            public Int64 Exp;
            public Int16 SPX;
            public Int16 SPY;

            public STRUCT_SCORE BaseScore;
            public STRUCT_SCORE CurrentScore;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_EQUIP)]
            public STRUCT_ITEM[] Equip;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_CARRY)]
            public STRUCT_ITEM[] Carry;

            public UInt32 LearnedSkill;
            public UInt32 Magic;

            public UInt16 ScoreBonus;
            public UInt16 SpecialBonus;
            public UInt16 SkillBonus;

            public byte Critical;
            public byte SaveMana;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] SkillBar;

            public byte GuildLevel;
            public UInt16 RegenHP;
            public UInt16 RegenMP;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Resist;

            public UInt16 Jewel;
            public Int16 EMPY;
            public Int16 EMPY2;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct STRUCT_ITEMLIST_AFFECT
        {
            public Int16 sEffect;
            public Int16 sValue;
        }

        //[StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct STRUCT_ITEMLIST_ORIGIN
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ITEMNAME_LENGTH)]
            public string Name;
            public Int16 IndexMesh;
            public Int16 IndexTexture;
            public Int16 IndexVisualEffect;
            public Int16 ReqLvl;
            public Int16 ReqStr;
            public Int16 ReqInt;
            public Int16 ReqDex;
            public Int16 ReqCon;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_STATICEFFECT)]
            public STRUCT_ITEMLIST_AFFECT[] stEffect;

            public Int32 Price;
            public Int16 nUnique;
            public Int32 nPos;
            public Int16 Extra;
            public Int16 Grade;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct STRUCT_ITEMLIST
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_ITEMLIST)]
            public STRUCT_ITEMLIST_ORIGIN[] Item;
        }
    }
}
