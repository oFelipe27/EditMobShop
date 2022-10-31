using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MOBEditor.Structs;

namespace MOBEditor
{
    public class Controller
    {
        public static List<STRUCT_MOB> ListMob = new List<STRUCT_MOB>();
        public static List<STRUCT_MOB> ListNpcShop = new List<STRUCT_MOB>();
        public static STRUCT_ITEMLIST pItemList = new STRUCT_ITEMLIST();

        public static string CurrentDir = "npc";

        public static void ReadMOB()
        {
            ListMob.Clear();
            string dir = CurrentDir;

            var fileEntries = Directory.GetFiles(dir);
            foreach (var patchfile in fileEntries)
            {
                var mob = LoadFile<STRUCT_MOB>(patchfile);

                if (mob.MobName == null)
                    continue;

                if (mob.Merchant == 1 || mob.Merchant == 19)
                    ListNpcShop.Add(mob);

                ListMob.Add(mob);

                //if (mob.Merchant == 1 || mob.Merchant == 19)
                //    ListNpcShop.Add(mob);
                //else
                //    ListMob.Add(mob);
            }
        }

        public static void ReadItemList()
        {
            string patch = "ItemList.bin";

            byte[] data = File.ReadAllBytes(patch);

            for (Int32 i = 0; i < data.Length; ++i)
            {
                data[i] ^= 0x5A;
            }

            pItemList = LoadFile<STRUCT_ITEMLIST>(data);
        }

        public static T LoadFile<T>(string patch) where T : struct
        {
            if (!File.Exists(patch))
            {
                MessageBox.Show($"{patch} not found!", "Error");
                return new T();
            }

            byte[] rawData = File.ReadAllBytes(patch);

            var pinnedRawData = GCHandle.Alloc(rawData, GCHandleType.Pinned);
            try
            {
                var pinnedRawDataPtr = pinnedRawData.AddrOfPinnedObject();
                return (T)Marshal.PtrToStructure(pinnedRawDataPtr, typeof(T));
            }
            finally
            {
                pinnedRawData.Free();
            }
        }

        public static T LoadFile<T>(byte[] rawData) where T : struct
        {
            var pinnedRawData = GCHandle.Alloc(rawData, GCHandleType.Pinned);
            try
            {
                var pinnedRawDataPtr = pinnedRawData.AddrOfPinnedObject();
                return (T)Marshal.PtrToStructure(pinnedRawDataPtr, typeof(T));
            }
            finally
            {
                pinnedRawData.Free();
            }
        }

        public static void SaveFile<T>(T bufffer, string patch)
        {
            try
            {
                if (patch == string.Empty)
                    return;

                byte[] arr = new byte[Marshal.SizeOf(bufffer)];

                IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(bufffer));
                Marshal.StructureToPtr(bufffer, ptr, false);
                Marshal.Copy(ptr, arr, 0, Marshal.SizeOf(bufffer));
                Marshal.FreeHGlobal(ptr);

                File.WriteAllBytes(patch, arr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save file error.");
            }
        }
    }
}
