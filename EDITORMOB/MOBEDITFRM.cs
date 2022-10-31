using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TextBox = System.Windows.Forms.TextBox;

namespace MOBEditor
{
    public partial class FormEditor : Form
    {
        public static Point[] WritePoint = new Point[27];
        public static Point[] ReadPoint = new Point[27];

        public static Point[] OriginalReadPoint = new Point[64];
        public static Point[] OriginalWritePoint = new Point[64];

        public FormEditor()
        {
            InitializeComponent();

            Controller.ReadItemList();
            InitiMobList();

            if (Directory.Exists("account"))
            {
                ComboFolder.Items.Add("BaseMob");
                return;
            }

            string[] folders =
            {
                "npc", "Boss", "BaseSummon",
                "Agua N", "Agua M", "Agua A",
                "Pesa N", "Pesa M", "Pesa A",
                "Duelo N", "Duelo M", "Duelo A"
            };

            ComboFolder.DataSource = folders;
            ComboFolder.SelectedText = folders[0];

            SetLocationInfo();
        }

        private void SetLocationInfo()
        {
            foreach (Control textbox in GroupCarry.Controls)
            {
                if (textbox is TextBox)
                {
                    var index = textbox.TabIndex;
                    if (index >= 0 && index < 5)
                    {
                        if (textbox.TabStop)
                            WritePoint[index] = textbox.Location;
                        else
                            ReadPoint[index] = textbox.Location;
                    }
                    if (index >= 8 && index <= 12)
                    {
                        if (textbox.TabStop)
                            WritePoint[index - 3] = textbox.Location;
                        else
                            ReadPoint[index - 3] = textbox.Location;
                    }
                    if (index >= 16 && index <= 20)
                    {
                        if (textbox.TabStop)
                            WritePoint[index - 6] = textbox.Location;//15
                        else
                            ReadPoint[index - 6] = textbox.Location;//15
                    }
                    if (index >= 24 && index <= 28)
                    {
                        if (textbox.TabStop)
                            WritePoint[index - 9] = textbox.Location;//20
                        else
                            ReadPoint[index - 9] = textbox.Location;
                    }
                    if (index >= 32 && index <= 36)
                    {
                        if (textbox.TabStop)
                            WritePoint[index - 12] = textbox.Location;//25
                        else
                            ReadPoint[index - 12] = textbox.Location;
                    }
                    if (index >= 40 && index <= 41)
                    {
                        if (textbox.TabStop)
                            WritePoint[index - 15] = textbox.Location;//20
                        else
                            ReadPoint[index - 15] = textbox.Location;
                    }

                    if (textbox.TabStop)
                        OriginalWritePoint[index] = textbox.Location;
                    else
                        OriginalReadPoint[index] = textbox.Location;
                }
            }
        }

        private void InitiMobList()
        {
            Controller.ReadMOB();
            ClearMobList();
            foreach (var mob in (BtnChangeMode.Text == "EDITMOB") ? Controller.ListNpcShop : Controller.ListMob)
            {
                AddMobForList(mob.MobName);
            }
        }

        public void ClearMobList()
        {
            ListNpc.Items.Clear();
        }

        public void AddMobForList(string mobname)
        {
            ListNpc.Items.Add(mobname);
        }

        private void MakeTBox(TextBox tb, object obj)
        {
            tb.Text = obj.ToString();
        }

        private T GetMobInfo<T>(TextBox tb)
        {
            return (T)Convert.ChangeType(tb.Text, typeof(T));
        }

        private void MakeItemImBox(Structs.STRUCT_ITEM[] pItem, bool isCarry)
        {
            foreach (Control textbox in (isCarry) ? GroupCarry.Controls : GroupEquip.Controls)
            {
                if (!(textbox is TextBox))
                    continue;

                Int32 pos = textbox.TabIndex;

                if (isCarry && pos < 27 && BtnChangeMode.Text == "EDITMOB")
                {
                    pos = (textbox.TabIndex % 9) + ((textbox.TabIndex / 9) * 27);
                }

                var item = pItem[pos];

                if (!textbox.TabStop)
                {
                    MakeTBox((TextBox)textbox, Controller.pItemList.Item[item.sIndex].Name);
                    continue;
                }

                string szItem = $"{item.sIndex} ";

                for (Int32 i = 0; i < 3; ++i)
                {
                    if (i != 2)
                    {
                        szItem += string.Format("{0} {1} ", item.stEffect[i].cEffect, item.stEffect[i].cValue);
                        continue;
                    }

                    szItem += string.Format("{0} {1}", item.stEffect[i].cEffect, item.stEffect[i].cValue);
                }

                MakeTBox((TextBox)textbox, szItem);
            }
        }

        private void MakeItemForBox(Structs.STRUCT_ITEM[] pItem, bool isCarry)
        {
            foreach (Control textbox in (isCarry) ? GroupCarry.Controls : GroupEquip.Controls)
            {
                if (!(textbox is TextBox))
                    continue;

                if (!textbox.TabStop)
                    continue;

                Int32 pos = textbox.TabIndex;

                if (isCarry && pos < 27 && BtnChangeMode.Text == "EDITMOB")
                {
                    pos = (textbox.TabIndex % 9) + ((textbox.TabIndex / 9) * 27);
                }

                var szItem = textbox.Text.Split(' ');

                pItem[pos].Clear();

                pItem[pos].sIndex = Convert.ToInt16(szItem[0]);

                for (Int32 i = 0, j = 1; i < 3; ++i, j += 2)
                {
                    if (j < szItem.Length)
                        pItem[pos].stEffect[i].cEffect = Convert.ToByte(szItem[j]);

                    if ((j + 1) < szItem.Length)
                        pItem[pos].stEffect[i].cValue = Convert.ToByte(szItem[j + 1]);
                }
            }
        }

        private void ListNpc_SelectedIndexChanged(object sender, EventArgs e)
        {
            Int32 index = ListNpc.SelectedIndex;
            if (index == -1)
                return;

            var mob = (BtnChangeMode.Text == "EDITSHOP") ? Controller.ListMob[index] : Controller.ListNpcShop[index];

            MakeTBox(tbName, mob.MobName);
            MakeTBox(tbRace, mob.Clan);
            MakeTBox(tbMerchant, mob.Merchant);
            MakeTBox(tbExp, mob.Exp);
            MakeTBox(tbCoin, mob.Coin);
            MakeTBox(tbSPX, mob.SPX);
            MakeTBox(tbSPY, mob.SPY);
            MakeTBox(tbLearn, mob.LearnedSkill);
            MakeTBox(tbRegenHp, mob.RegenHP);
            MakeTBox(tbRegenMp, mob.RegenMP);
            MakeTBox(tbDirection, mob.BaseScore.AttackRun);
            MakeTBox(tbBonus, mob.ScoreBonus);
            tbSkill.Text = String.Format("{0} {1} {2} {3}", mob.SkillBar[0], mob.SkillBar[1], mob.SkillBar[2], mob.SkillBar[3]);
            tbResist.Text = String.Format("{0} {1} {2} {3}", mob.Resist[0], mob.Resist[1], mob.Resist[2], mob.Resist[3]);

            MakeItemImBox(mob.Equip, false); // print equip array in text box
            MakeItemImBox(mob.Carry, true); // print equip array in text box

            MakeTBox(tbLevel, mob.BaseScore.Level);
            MakeTBox(tbAc, mob.BaseScore.Ac);
            MakeTBox(tbDam, mob.BaseScore.Damage);
            MakeTBox(tbRegen, mob.BaseScore.ChaosRate); // ??

            MakeTBox(tbStr, mob.BaseScore.Str);
            MakeTBox(tbDex, mob.BaseScore.Dex);
            MakeTBox(tbInt, mob.BaseScore.Int);
            MakeTBox(tbCon, mob.BaseScore.Con);

            MakeTBox(tbHp, mob.BaseScore.Hp);
            MakeTBox(tbMaxHp, mob.BaseScore.MaxHp);
            MakeTBox(tbMp, mob.BaseScore.Mp);
            MakeTBox(tbMaxMp, mob.BaseScore.MaxMp);

            MakeTBox(tbNear, mob.BaseScore.Special[0]);
            MakeTBox(tbNearMotion, mob.BaseScore.Special[1]);
            MakeTBox(tbFar, mob.BaseScore.Special[2]);
            MakeTBox(tbFarMotion, mob.BaseScore.Special[3]);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            Int32 index = ListNpc.SelectedIndex;
            if (index == -1)
                return;

            var mob = (BtnChangeMode.Text == "EDITSHOP") ? Controller.ListMob[index] : Controller.ListNpcShop[index];

            mob.MobName = GetMobInfo<string>(tbName);
            mob.Clan = GetMobInfo<byte>(tbRace);
            mob.Merchant = GetMobInfo<byte>(tbMerchant);
            mob.Exp = GetMobInfo<Int64>(tbExp);
            mob.Coin = GetMobInfo<Int32>(tbCoin);

            mob.SPX = GetMobInfo<Int16>(tbSPX);
            mob.SPY = GetMobInfo<Int16>(tbSPY);
            mob.LearnedSkill = GetMobInfo<UInt32>(tbLearn);
            mob.RegenHP = GetMobInfo<UInt16>(tbRegenHp);

            mob.RegenHP = GetMobInfo<UInt16>(tbRegenHp);
            mob.RegenMP = GetMobInfo<UInt16>(tbRegenMp);
            mob.BaseScore.AttackRun = GetMobInfo<byte>(tbDirection);
            mob.ScoreBonus = GetMobInfo<UInt16>(tbBonus);

            string[] szResist = tbResist.Text.Split(' ');
            for (Int32 i = 0; i < 4; ++i)
                mob.Resist[i] = Convert.ToByte(szResist[i]);

            string[] szSkill = tbSkill.Text.Split(' ');
            for (Int32 i = 0; i < 4; ++i)
                mob.SkillBar[i] = Convert.ToByte(szSkill[i]);

            mob.BaseScore.Level = GetMobInfo<Int32>(tbLevel);
            mob.BaseScore.Ac = GetMobInfo<Int32>(tbAc);
            mob.BaseScore.Damage = GetMobInfo<Int32>(tbDam);
            mob.BaseScore.ChaosRate = GetMobInfo<byte>(tbRegen);

            mob.BaseScore.Str = GetMobInfo<Int16>(tbStr);
            mob.BaseScore.Dex = GetMobInfo<Int16>(tbDex);
            mob.BaseScore.Int = GetMobInfo<Int16>(tbInt);
            mob.BaseScore.Con = GetMobInfo<Int16>(tbCon);

            mob.BaseScore.Hp = GetMobInfo<Int32>(tbHp);
            mob.BaseScore.MaxHp = GetMobInfo<Int32>(tbMaxHp);
            mob.BaseScore.Mp = GetMobInfo<Int32>(tbMp);
            mob.BaseScore.MaxMp = GetMobInfo<Int32>(tbMaxMp);

            mob.BaseScore.Special[0] = GetMobInfo<Int16>(tbNear);
            mob.BaseScore.Special[1] = GetMobInfo<Int16>(tbNearMotion);
            mob.BaseScore.Special[2] = GetMobInfo<Int16>(tbFar);
            mob.BaseScore.Special[3] = GetMobInfo<Int16>(tbFarMotion);

            MakeItemForBox(mob.Equip, false);
            MakeItemForBox(mob.Carry, true);

            mob.CurrentScore = mob.BaseScore;

            if (BtnChangeMode.Text == "EDITSHOP")
                Controller.ListMob[index] = mob;
            else
                Controller.ListNpcShop[index] = mob;

            SetCurrentDir();

            string backDir = Controller.CurrentDir;
            Controller.CurrentDir += mob.MobName;

            Controller.SaveFile(mob, Controller.CurrentDir);
            MakeLog(string.Format("mob [{0}] save success", mob.MobName));
            Controller.CurrentDir = backDir;

            InitiMobList();

            Int32 nIndex = ListNpc.FindString(mob.MobName);
            if (nIndex != -1)
            {
                ListNpc.SetSelected(nIndex, true);
            }
        }

        private void SetCurrentDir()
        {
            Controller.CurrentDir = string.Format(@"NpcExtra\{0}\", ComboFolder.Text);
            if (Controller.CurrentDir == "NpcExtra\\npc\\")
                Controller.CurrentDir = "npc\\";

            else if (Controller.CurrentDir == "NpcExtra\\BaseSummon\\")
                Controller.CurrentDir = "BaseSummon\\";
        }

        private void BtnReload_Click(object sender, EventArgs e)
        {
            SetCurrentDir();
            InitiMobList();
        }

        public void MakeLog(string text)
        {
            this.Text = text;
        }

        private void ComponentResize()
        {
            if (BtnChangeMode.Text == "EDITSHOP")
            {
                BtnChangeMode.Text = "EDITMOB";
                lbRate1.Text = "1ª Fileira";
                lbRate2.Text = "2ª Fileira";
                lbRate3.Text = "3ª Fileira";
                lbRate4.Text = "4ª Fileira";
                lbRate5.Text = "5ª Fileira";
                lbRate6.Text = "6ª Fileira";

                lbRate4.Location = new Point(72, 140);
                lbRate5.Location = new Point(318, 140);
                lbRate6.Location = new Point(559, 140);

                ListNpc.Size = new Size(130, 400);
                GroupCarry.Size = new Size(743, 270);

                this.Size = new Size(1085, 450);
            }
            else
            {
                BtnChangeMode.Text = "EDITSHOP";
                lbRate1.Text = "1 / 1000";
                lbRate2.Text = "1 / 1000";
                lbRate3.Text = "1 / 1000";
                lbRate4.Text = "1 / 2000";
                lbRate5.Text = "1 / 2000";
                lbRate6.Text = "1 / 2000";

                lbRate4.Location = new Point(72, 204);
                lbRate5.Location = new Point(318, 204);
                lbRate6.Location = new Point(559, 204);

                ListNpc.Size = new Size(130, 680);
                GroupCarry.Size = new Size(743, 582);
                this.Size = new Size(1085, 815);
            }
        }

        private void BtnChangeMode_Click(object sender, EventArgs e)
        {
            if (BtnChangeMode.Text == "EDITSHOP")
            {
                foreach (Control textbox in GroupCarry.Controls)
                {
                    if (!(textbox is TextBox))
                        continue;

                    if (textbox.TabIndex >= 27)
                    {
                        textbox.Visible = false;
                        continue;
                    }

                    if (textbox.TabStop)
                        textbox.Location = WritePoint[textbox.TabIndex];
                    else
                        textbox.Location = ReadPoint[textbox.TabIndex];

                    if (textbox.TabIndex >= 15)
                    {
                        textbox.Location = new Point(textbox.Location.X, textbox.Location.Y - 62);
                    }
                }
            }
            else
            {
                foreach (Control textbox in GroupCarry.Controls)
                {
                    if (!(textbox is TextBox))
                        continue;

                    textbox.Visible = true;

                    if (textbox.TabStop)
                        textbox.Location = OriginalWritePoint[textbox.TabIndex];
                    else
                        textbox.Location = OriginalReadPoint[textbox.TabIndex];
                }
            }

            ComponentResize();
            InitiMobList();
        }

        private void tbSearchMob_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbSearchMob.Text))
                return;

            Int32 index = ListNpc.FindString(tbSearchMob.Text);
            if (index == -1)
                return;

            ListNpc.SetSelected(index, true);
        }

        private void tbSearchMob_Click(object sender, EventArgs e)
        {
            if (tbSearchMob.Text == "Search Mob")
                tbSearchMob.Text = String.Empty;
        }

        private void ChangeText(object sender, KeyPressEventArgs e)
        {
            char[] authKeys = { ' ', '\b', '\u0001', '\u0003', '\u0016', '\u0018' };
            bool autch = false;

            foreach (char c in authKeys)
            {
                if (e.KeyChar == c)
                {
                    autch = true;
                    break;
                }
            }

            //liberado numeros, ctrl + a, e espaço
            if (!char.IsDigit(e.KeyChar) && !autch)
            {
                e.Handled = true;
                return;
            }
        }

        private void TextChange(object sender, EventArgs e)
        {
            var textbox = sender as TextBox;
            var tbName = GroupCarry.Controls.OfType<TextBox>().First(c => c.TabIndex == textbox.TabIndex && c.TabStop == false);

            var szItemName = textbox.Text.Split(' ');
            if (szItemName != null && Regex.IsMatch(szItemName[0], @"^[0-9]+$"))
            {
                var itemId = Convert.ToInt16(szItemName[0]);
                tbName.Text = Controller.pItemList.Item[itemId].Name;
            }
        }
    }
}
