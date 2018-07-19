using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using CitizenFX.Core.Native;
using System.Drawing;

namespace MapEditor
{
    public static class Utils
    {
        public static Vector3 RotationToDirection(Vector3 rotation)
        {
            double retZ = rotation.Z * 0.01745329f;
            double retX = rotation.X * 0.01745329f;
            double absX = Math.Abs(Math.Cos(retX));
            Vector3 DirectionVector = new Vector3((float)-(Math.Sin(retZ) * absX), (float)(Math.Cos(retZ) * absX), (float)Math.Sin(retX));
            DirectionVector.Normalize();
            return DirectionVector;
        }
        public static Vector3 RightVector(Vector3 vec)
        {
            Vector3 RightVector = Vector3.Cross(vec, new Vector3(0f, 0f, 1f));
            RightVector.Normalize();
            return RightVector;
        }

        public static String InstructionalBtnFromControl(Control c)
        {
            return Function.Call<String>(Hash.GET_CONTROL_INSTRUCTIONAL_BUTTON, 1, c, true);
        }
        public static bool IsEntityAPed(Entity e)
        {
            return Function.Call<bool>(Hash.IS_ENTITY_A_PED, e);
        }
        public static bool IsEntityAVehicle(Entity e)
        {
            return Function.Call<bool>(Hash.IS_ENTITY_A_VEHICLE, e);
        }
        public static bool IsEntityAnObject(Entity e)
        {
            return Function.Call<bool>(Hash.IS_ENTITY_AN_OBJECT, e);
        }

        public static void DrawText(String txt, float xPos, float yPos, int r, int g, int b)
        {
            Function.Call(Hash.SET_TEXT_FONT, 0);
            Function.Call(Hash.SET_TEXT_SCALE, 0.3f, 0.3f);
            Function.Call(Hash.SET_TEXT_COLOUR, r, g, b, 255);
            Function.Call(Hash.SET_TEXT_CENTRE, 1);
            Function.Call(Hash._SET_TEXT_ENTRY, "STRING");
            Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, txt);
            Function.Call(Hash._DRAW_TEXT, xPos, yPos);
        }
        public static void DrawEntityBox(Entity ent, Color color) // from Guadmaz's map editor
        {
            // Following this naming https://www.ilemaths.net/img/forum_img/0550/forum_550205_1.png
            Vector3 minDim, maxDim;
            ent.Model.GetDimensions(out minDim, out maxDim);
            Vector3 a = ent.GetOffsetPosition(new Vector3(minDim.X, maxDim.Y, minDim.Z));
            Vector3 b = ent.GetOffsetPosition(new Vector3(minDim.X, minDim.Y, minDim.Z));
            Vector3 c = ent.GetOffsetPosition(new Vector3(maxDim.X, minDim.Y, minDim.Z));
            Vector3 d = ent.GetOffsetPosition(new Vector3(maxDim.X, maxDim.Y, minDim.Z));
            Vector3 e = ent.GetOffsetPosition(new Vector3(minDim.X, maxDim.Y, maxDim.Z));
            Vector3 f = ent.GetOffsetPosition(new Vector3(minDim.X, minDim.Y, maxDim.Z));
            Vector3 g = ent.GetOffsetPosition(new Vector3(maxDim.X, minDim.Y, maxDim.Z));
            Vector3 h = ent.GetOffsetPosition(new Vector3(maxDim.X, maxDim.Y, maxDim.Z));
            //Ground plane
            World.DrawLine(a, b, color);
            World.DrawLine(b, c, color);
            World.DrawLine(c, d, color);
            World.DrawLine(d, a, color);
            //Top plane
            World.DrawLine(e, f, color);
            World.DrawLine(f, g, color);
            World.DrawLine(g, h, color);
            World.DrawLine(h, e, color);
            //Connecting edges
            World.DrawLine(a, e, color);
            World.DrawLine(b, f, color);
            World.DrawLine(c, g, color);
            World.DrawLine(d, h, color);
        }
        public static async Task<string> GetCustomUserInput(string windowTitle, string defaultText, int maxLength)
        {
            string result = String.Empty;
            Function.Call(Hash.ADD_TEXT_ENTRY, "MM_WT", windowTitle);
            Function.Call(Hash.DISPLAY_ONSCREEN_KEYBOARD, true, "MM_WT", "", defaultText, "", "", "", maxLength + 1);
            while (Function.Call<int>(Hash.UPDATE_ONSCREEN_KEYBOARD) == 0)
            {
                await BaseScript.Delay(0);
            }
            if (Function.Call<int>(Hash.UPDATE_ONSCREEN_KEYBOARD) == 1)
            {
                result = Function.Call<string>(Hash.GET_ONSCREEN_KEYBOARD_RESULT);
            }
            return result;

        }

        public static Vector3 MultMatrixVec(Matrix3x3 m, Vector3 v)
        {
            Vector3 vec = new Vector3();
            vec.X = Vector3.Dot(m.Row1, v);
            vec.Y = Vector3.Dot(m.Row2, v);
            vec.Z = Vector3.Dot(m.Row3, v);
            return vec;
        }
        public static string func_3999(int iParam0)
        {
            if (iParam0 == Game.GenerateHash("prop_const_fence02b"))
            {
                return "FMMC_PR_0";
            }
            if (iParam0 == 2084346858)
            {
                return "FMMC_PR_1";
            }
            if (iParam0 == -1601837956)
            {
                return "FMMC_PR_2";
            }
            if (iParam0 == Game.GenerateHash("prop_offroad_bale01"))
            {
                return "FMMC_PR_3";
            }
            if (iParam0 == Game.GenerateHash("prop_offroad_tyres01"))
            {
                return "FMMC_PR_4";
            }
            if (iParam0 == 812376260)
            {
                return "FMMC_PR_5";
            }
            if (iParam0 == -1286880215)
            {
                return "FMMC_PR_6";
            }
            if (iParam0 == 1172303719)
            {
                return "FMMC_PR_7";
            }
            if (iParam0 == Game.GenerateHash("prop_barier_conc_05a"))
            {
                return "FMMC_PR_8";
            }
            if (iParam0 == Game.GenerateHash("prop_barier_conc_05b"))
            {
                return "FMMC_PR_9";
            }
            if (iParam0 == -1228223417)
            {
                return "FMMC_PR_10";
            }
            if (iParam0 == Game.GenerateHash("prop_bench_05"))
            {
                return "FMMC_PR_12";
            }
            if (iParam0 == -71417349)
            {
                return "FMMC_PR_13";
            }
            if (iParam0 == 1805980844)
            {
                return "FMMC_PR_14";
            }
            if (iParam0 == -403891623)
            {
                return "FMMC_PR_BNBLUE";
            }
            if (iParam0 == Game.GenerateHash("prop_dock_bouy_1"))
            {
                return "FMMC_PR_15";
            }
            if (iParam0 == Game.GenerateHash("prop_dock_bouy_2"))
            {
                return "FMMC_PR_16";
            }
            if (iParam0 == Game.GenerateHash("prop_elecbox_24"))
            {
                return "FMMC_PR_17";
            }
            if (iParam0 == Game.GenerateHash("prop_elecbox_24b"))
            {
                return "FMMC_PR_18";
            }
            if (iParam0 == Game.GenerateHash("prop_portacabin01"))
            {
                return "FMMC_PR_19";
            }
            if (iParam0 == 1899123601)
            {
                return "FMMC_PR_20";
            }
            if (iParam0 == -1951226014)
            {
                return "FMMC_PR_21";
            }
            if (iParam0 == Game.GenerateHash("prop_cons_crate"))
            {
                return "FMMC_PR_22";
            }
            if (iParam0 == Game.GenerateHash("prop_jyard_block_01a"))
            {
                return "FMMC_PR_23";
            }
            if (iParam0 == Game.GenerateHash("prop_conc_sacks_02a"))
            {
                return "FMMC_PR_24";
            }
            if (iParam0 == Game.GenerateHash("prop_byard_sleeper01"))
            {
                return "FMMC_PR_25";
            }
            if (iParam0 == Game.GenerateHash("prop_shuttering01"))
            {
                return "FMMC_PR_26";
            }
            if (iParam0 == Game.GenerateHash("prop_shuttering02"))
            {
                return "FMMC_PR_27";
            }
            if (iParam0 == Game.GenerateHash("prop_shuttering03"))
            {
                return "FMMC_PR_28";
            }
            if (iParam0 == Game.GenerateHash("prop_shuttering04"))
            {
                return "FMMC_PR_29";
            }
            if (iParam0 == 1367246936)
            {
                return "FMMC_PR_30";
            }
            if (iParam0 == 1861370687)
            {
                return "FMMC_PR_31";
            }
            if (iParam0 == Game.GenerateHash("prop_rub_cont_01b"))
            {
                return "FMMC_PR_32";
            }
            if (iParam0 == Game.GenerateHash("prop_rail_boxcar4"))
            {
                return "FMMC_PR_33";
            }
            if (iParam0 == Game.GenerateHash("prop_rub_railwreck_2"))
            {
                return "FMMC_PR_34";
            }
            if (iParam0 == 874602658)
            {
                return "FMMC_PR_35";
            }
            if (iParam0 == Game.GenerateHash("prop_container_ld2"))
            {
                return "FMMC_PR_36";
            }
            if (iParam0 == Game.GenerateHash("prop_rail_boxcar"))
            {
                return "FMMC_PR_37";
            }
            if (iParam0 == Game.GenerateHash("prop_rail_boxcar3"))
            {
                return "FMMC_PR_38";
            }
            if (iParam0 == Game.GenerateHash("prop_byard_floatpile"))
            {
                return "FMMC_PR_39";
            }
            if (iParam0 == -1726996371)
            {
                return "FMMC_PR_40";
            }
            if (iParam0 == Game.GenerateHash("prop_watercrate_01"))
            {
                return "FMMC_PR_41";
            }
            if (iParam0 == 1165008631)
            {
                return "FMMC_PR_42";
            }
            if (iParam0 == -2022916910)
            {
                return "FMMC_PR_43";
            }
            if (iParam0 == -1322183878)
            {
                return "FMMC_PR_44";
            }
            if (iParam0 == Game.GenerateHash("prop_cash_crate_01"))
            {
                return "FMMC_PR_45";
            }
            if (iParam0 == Game.GenerateHash("prop_bin_13a"))
            {
                return "FMMC_PR_46";
            }
            if (iParam0 == Game.GenerateHash("prop_bin_14a"))
            {
                return "FMMC_PR_47";
            }
            if (iParam0 == Game.GenerateHash("prop_dumpster_3a"))
            {
                return "FMMC_PR_48";
            }
            if (iParam0 == Game.GenerateHash("prop_dumpster_4b"))
            {
                return "FMMC_PR_49";
            }
            if (iParam0 == 218085040)
            {
                return "FMMC_PR_50";
            }
            if (iParam0 == Game.GenerateHash("prop_skip_06a"))
            {
                return "FMMC_PR_51";
            }
            if (iParam0 == -2138350253)
            {
                return "FMMC_PR_52";
            }
            if (iParam0 == Game.GenerateHash("prop_elecbox_16"))
            {
                return "FMMC_PR_53";
            }
            if (iParam0 == Game.GenerateHash("prop_elecbox_14"))
            {
                return "FMMC_PR_54";
            }
            if (iParam0 == Game.GenerateHash("prop_ind_deiseltank"))
            {
                return "FMMC_PR_55";
            }
            if (iParam0 == Game.GenerateHash("prop_ind_mech_02a"))
            {
                return "FMMC_PR_56";
            }
            if (iParam0 == Game.GenerateHash("prop_ind_mech_02b"))
            {
                return "FMMC_PR_57";
            }
            if (iParam0 == Game.GenerateHash("prop_sub_trans_01a"))
            {
                return "FMMC_PR_58";
            }
            if (iParam0 == Game.GenerateHash("prop_sub_trans_02a"))
            {
                return "FMMC_PR_59";
            }
            if (iParam0 == Game.GenerateHash("prop_sub_trans_04a"))
            {
                return "FMMC_PR_60";
            }
            if (iParam0 == Game.GenerateHash("prop_skip_04"))
            {
                return "FMMC_PR_62";
            }
            if (iParam0 == Game.GenerateHash("prop_mp_ramp_01") || iParam0 == -1359996601)
            {
                return "FMMC_PR_61";
            }
            if (iParam0 == Game.GenerateHash("prop_mp_ramp_02") || iParam0 == -1061569318)
            {
                return "FMMC_PR_63";
            }
            if (iParam0 == Game.GenerateHash("prop_mp_ramp_03") || iParam0 == 1290523964)
            {
                return "FMMC_PR_64";
            }
            if (iParam0 == Game.GenerateHash("prop_skip_08a"))
            {
                return "FMMC_PR_65";
            }
            if (iParam0 == Game.GenerateHash("prop_jetski_ramp_01"))
            {
                return "FMMC_PR_66";
            }
            if (iParam0 == 1560354582)
            {
                return "FMMC_PR_67";
            }
            if (iParam0 == -1097090961)
            {
                return "FMMC_PR_68";
            }
            if (iParam0 == 240277467)
            {
                return "FMMC_PR_69";
            }
            if (iParam0 == -733912134)
            {
                return "FMMC_PR_70";
            }
            if (iParam0 == -1333126068)
            {
                return "FMMC_PR_71";
            }
            if (iParam0 == -1454939221)
            {
                return "FMMC_PR_72";
            }
            if (iParam0 == -1547577184)
            {
                return "FMMC_PR_73";
            }
            if (iParam0 == 219009290)
            {
                return "FMMC_PR_74";
            }
            if (iParam0 == 532969079)
            {
                return "FMMC_PR_75";
            }
            if (iParam0 == -1651641860)
            {
                return "FMMC_PR_76";
            }
            if (iParam0 == 793482617)
            {
                return "FMMC_PR_77";
            }
            if (iParam0 == Game.GenerateHash("prop_food_van_01"))
            {
                return "FMMC_PR_78";
            }
            if (iParam0 == Game.GenerateHash("prop_food_van_02"))
            {
                return "FMMC_PR_79";
            }
            if (iParam0 == -531344027)
            {
                return "FMMC_PR_80";
            }
            if (iParam0 == Game.GenerateHash("prop_truktrailer_01a"))
            {
                return "FMMC_PR_81";
            }
            if (iParam0 == Game.GenerateHash("prop_rub_buswreck_01"))
            {
                return "FMMC_PR_82";
            }
            if (iParam0 == Game.GenerateHash("prop_rub_buswreck_06"))
            {
                return "FMMC_PR_83";
            }
            if (iParam0 == 1069797899)
            {
                return "FMMC_PR_84";
            }
            if (iParam0 == 1434516869)
            {
                return "FMMC_PR_85";
            }
            if (iParam0 == -896997473)
            {
                return "FMMC_PR_86";
            }
            if (iParam0 == 1120812170)
            {
                return "FMMC_PR_87";
            }
            if (iParam0 == Game.GenerateHash("prop_shamal_crash"))
            {
                return "FMMC_PR_89";
            }
            if (iParam0 == 1925170211)
            {
                return "FMMC_PR_90";
            }
            if (iParam0 == -1305574636)
            {
                return "FMMC_PR_91";
            }
            if (iParam0 == -1913970053)
            {
                return "FMMC_PR_92";
            }
            if (iParam0 == 1475688080)
            {
                return "FMMC_PR_93";
            }
            if (iParam0 == 1753330262)
            {
                return "FMMC_PR_111";
            }
            if (iParam0 == -1600413027)
            {
                return "FMMC_PR_112";
            }
            if (iParam0 == -1683613150)
            {
                return "FMMC_PR_114";
            }
            if (iParam0 == -2115173136)
            {
                return "FMMC_PR_115";
            }
            if (iParam0 == 1390188982)
            {
                return "FMMC_PR_116";
            }
            if (iParam0 == 202174981)
            {
                return "FMMC_PR_94";
            }
            if (iParam0 == -28420472)
            {
                return "FMMC_PR_95";
            }
            if (iParam0 == -259572998)
            {
                return "FMMC_PR_96";
            }
            if (iParam0 == 1243328051)
            {
                return "FMMC_PR_97";
            }
            if (iParam0 == Game.GenerateHash("prop_sec_gate_01d"))
            {
                return "FMMC_PR_98";
            }
            if (iParam0 == Game.GenerateHash("prop_vault_shutter"))
            {
                return "FMMC_PR_100";
            }
            if (iParam0 == Game.GenerateHash("prop_fnclink_03gate5"))
            {
                return "FMMC_PR_FNCMGTS";
            }
            if (iParam0 == 1278261455)
            {
                return "FMMC_PR_FNCMGTD";
            }
            if (iParam0 == Game.GenerateHash("prop_plas_barier_01a"))
            {
                return "FMMC_PR_BARPRED";
            }
            if (iParam0 == Game.GenerateHash("prop_barier_conc_02b"))
            {
                return "FMMC_PR_BARCANW";
            }
            if (iParam0 == 765541575)
            {
                return "FMMC_PR_BARWRKP";
            }
            if (iParam0 == -565797937)
            {
                return "FMMC_PR_BARWRKW";
            }
            if (iParam0 == -1393524934)
            {
                return "FMMC_PR_FNCBWSG";
            }
            if (iParam0 == -837500542)
            {
                return "FMMC_PR_FNCBWLN";
            }
            if (iParam0 == 2122752615)
            {
                return "FMMC_PR_FNCBWWD";
            }
            if (iParam0 == -1314912103)
            {
                return "FMMC_PR_FNCBWWL";
            }
            if (iParam0 == 2074061472)
            {
                return "FMMC_PR_FNCCDCF";
            }
            if (iParam0 == -1593445012)
            {
                return "FMMC_PR_FNCCPDF";
            }
            if (iParam0 == -1880599759)
            {
                return "FMMC_PR_FNCCSCF";
            }
            if (iParam0 == -1894591898)
            {
                return "FMMC_PR_FNCCRDF";
            }
            if (iParam0 == -1519583462)
            {
                return "FMMC_PR_FNCCPRF";
            }
            if (iParam0 == 1916672189)
            {
                return "FMMC_PR_FNCCBPF";
            }
            if (iParam0 == -940719073)
            {
                return "FMMC_PR_FNCCTCF";
            }
            if (iParam0 == 1185366416)
            {
                return "FMMC_PR_FNCCSRF";
            }
            if (iParam0 == Game.GenerateHash("prop_gate_cult_01_l"))
            {
                return "FMMC_PR_FNCGTAL";
            }
            if (iParam0 == Game.GenerateHash("prop_gate_cult_01_r"))
            {
                return "FMMC_PR_FNCGTAR";
            }
            if (iParam0 == -1147467348)
            {
                return "FMMC_PR_BARQADB";
            }
            if (iParam0 == Game.GenerateHash("prop_const_fence02a"))
            {
                return "FMMC_PR_BARDUBU";
            }
            if (iParam0 == 2108146567)
            {
                return "FMMC_PR_BARSYSTEM::SINB";
            }
            if (iParam0 == Game.GenerateHash("prop_fncwood_16b"))
            {
                return "FMMC_PR_FNCPKOD";
            }
            if (iParam0 == Game.GenerateHash("prop_fncwood_16c"))
            {
                return "FMMC_PR_FNCPKOS";
            }
            if (iParam0 == 93794225)
            {
                return "FMMC_PR_FNCFMS";
            }
            if (iParam0 == 1322893877)
            {
                return "FMMC_PR_FNCFMSL";
            }
            if (iParam0 == -1178167275)
            {
                return "FMMC_PR_FNCFMD";
            }
            if (iParam0 == -872399736)
            {
                return "FMMC_PR_FNCFMT";
            }
            if (iParam0 == 373936410)
            {
                return "FMMC_PR_FNCFMSX";
            }
            if (iParam0 == 487569140)
            {
                return "FMMC_PR_BARGE";
            }
            if (iParam0 == 260517631)
            {
                return "FMMC_PR_CABTBTH";
            }
            if (iParam0 == 304890764)
            {
                return "FMMC_PR_CABPHUT";
            }
            if (iParam0 == -1186441238)
            {
                return "FMMC_PR_WODPLSM";
            }
            if (iParam0 == -740912282)
            {
                return "FMMC_PR_WODPLUT";
            }
            if (iParam0 == 1711856655)
            {
                return "FMMC_PR_CONCCND";
            }
            if (iParam0 == 1962326206)
            {
                return "FMMC_PR_CONCSAK";
            }
            if (iParam0 == Game.GenerateHash("prop_container_01mb"))
            {
                return "FMMC_PR_CNTLNGG";
            }
            if (iParam0 == Game.GenerateHash("prop_container_03mb"))
            {
                return "FMMC_PR_CNTSHTG";
            }
            if (iParam0 == -3872440)
            {
                return "FMMC_PR_PLTPILL";
            }
            if (iParam0 == -1853453107)
            {
                return "FMMC_PR_PLTPILS";
            }
            if (iParam0 == -2073573168)
            {
                return "FMMC_PR_CRTPILL";
            }
            if (iParam0 == 300547451)
            {
                return "FMMC_PR_BOXPILW";
            }
            if (iParam0 == 666561306)
            {
                return "FMMC_PR_DMPCLDB";
            }
            if (iParam0 == -58485588)
            {
                return "FMMC_PR_DMPCLDM";
            }
            if (iParam0 == -1333576134)
            {
                return "FMMC_PR_ELECBXW";
            }
            if (iParam0 == 1820092997)
            {
                return "FMMC_PR_ELCBXGN";
            }
            if (iParam0 == -1001532663)
            {
                return "FMMC_PR_ELCBXGY";
            }
            if (iParam0 == -57215983)
            {
                return "FMMC_PR_GANDLMP";
            }
            if (iParam0 == -1576911260)
            {
                return "FMMC_PR_FEEDER";
            }
            if (iParam0 == Game.GenerateHash("prop_skip_03"))
            {
                return "FMMC_PR_RMPDMPM";
            }
            if (iParam0 == -535359464)
            {
                return "FMMC_PR_RMPOLD";
            }
            if (iParam0 == -613845235)
            {
                return "FMMC_PR_RMPHP";
            }
            if (iParam0 == 1367199760)
            {
                return "FMMC_PR_RMPQP";
            }
            if (iParam0 == -1685045150)
            {
                return "FMMC_PR_RMPFRS";
            }
            if (iParam0 == -1907102305)
            {
                return "FMMC_PR_RMPLFR";
            }
            if (iParam0 == 1982829832)
            {
                return "FMMC_PR_RMPFRK";
            }
            if (iParam0 == -1912195761)
            {
                return "FMMC_PR_RMPFBP";
            }
            if (iParam0 == -1977592297)
            {
                return "FMMC_PR_RMPPILE";
            }
            if (iParam0 == -125664540)
            {
                return "FMMC_PR_FRMOLDT";
            }
            if (iParam0 == -1748303324)
            {
                return "FMMC_PR_WRKCRRD";
            }
            if (iParam0 == -2021659595)
            {
                return "FMMC_PR_WRKCCH";
            }
            if (iParam0 == -73584559)
            {
                return "FMMC_PR_NTROLVT";
            }
            if (iParam0 == -1279773008)
            {
                return "FMMC_PR_NTROAKT";
            }
            if (iParam0 == Game.GenerateHash("prop_tree_fallen_02"))
            {
                return "FMMC_PR_NTRFLNT";
            }
            if (iParam0 == 1731900299)
            {
                return "FMMC_PR_HBSTK";
            }
            if (iParam0 == 1395331371)
            {
                return "FMMC_PR_HBRND";
            }
            if (iParam0 == Game.GenerateHash("prop_haybale_02"))
            {
                return "FMMC_PR_HBSSTK";
            }
            if (iParam0 == Game.GenerateHash("prop_haybale_01"))
            {
                return "FMMC_PR_HBSML";
            }
            if (iParam0 == Game.GenerateHash("prop_byard_float_02"))
            {
                return "FMMC_PR_FLOATD";
            }
            if (iParam0 == 2090810892)
            {
                return "FMMC_PR_TYR1";
            }
            if (iParam0 == -905357089)
            {
                return "FMMC_PR_TYR2";
            }
            if (iParam0 == -666143389)
            {
                return "FMMC_PR_TYR3";
            }
            if (iParam0 == 1465709448)
            {
                return "FMMC_PR_TYR4";
            }
            if (iParam0 == 631705629)
            {
                return "FMMC_PR_TYR5";
            }
            if (iParam0 == -1082910619)
            {
                return "FMMC_PR_TYR1B";
            }
            if (iParam0 == 159919520)
            {
                return "FMMC_PR_TYR2B";
            }
            if (iParam0 == 690613779)
            {
                return "FMMC_PR_TYR3B";
            }
            if (iParam0 == 776861087)
            {
                return "FMMC_PR_TYR1C";
            }
            if (iParam0 == -160036996)
            {
                return "FMMC_PR_TYR2C";
            }
            if (iParam0 == -140899596)
            {
                return "FMMC_PR_TYR3C";
            }
            if (iParam0 == Game.GenerateHash("prop_offroad_tyres01"))
            {
                return "FMMC_PR_ORT1";
            }
            if (iParam0 == 812376260)
            {
                return "FMMC_PR_ORT2";
            }
            if (iParam0 == Game.GenerateHash("prop_pipes_conc_01"))
            {
                return "FMMC_PR_CNPPE";
            }
            if (iParam0 == 690751374)
            {
                return "FMMC_PR_SGTE";
            }
            if (iParam0 == -293536422)
            {
                return "FMMC_PR_SMTR";
            }
            if (iParam0 == 1012842044)
            {
                return "FMMC_PR_MUTR";
            }
            if (iParam0 == -1663557985)
            {
                return "FMMC_PR_SBLE";
            }
            if (iParam0 == 984170102)
            {
                return "FMMC_PR_BLEA";
            }
            if (iParam0 == 1275920395)
            {
                return "FMMC_PR_BEAF";
            }
            if (iParam0 == 54873101)
            {
                return "FMMC_PR_SEGE";
            }
            if (iParam0 == Game.GenerateHash("prop_air_blastfence_01"))
            {
                return "FMMC_PR_FNCBLST";
            }
            if (iParam0 == Game.GenerateHash("prop_mb_sandblock_01"))
            {
                return "FMMC_PR_SNDBKSI";
            }
            if (iParam0 == Game.GenerateHash("prop_mb_sandblock_02"))
            {
                return "FMMC_PR_SNDBKTR";
            }
            if (iParam0 == 1708971027)
            {
                return "FMMC_PR_SNDBKED";
            }
            if (iParam0 == Game.GenerateHash("prop_mb_sandblock_04"))
            {
                return "FMMC_PR_SNDBKCR";
            }
            if (iParam0 == 523344868)
            {
                return "FMMC_PR_SNDBKST";
            }
            if (iParam0 == Game.GenerateHash("prop_mb_hesco_06"))
            {
                return "FMMC_PR_SNDBKFT";
            }
            if (iParam0 == -679229497)
            {
                return "FMMC_PR_BARWDQU";
            }
            if (iParam0 == 169792355)
            {
                return "FMMC_PR_CABAPHT";
            }
            if (iParam0 == -105439435)
            {
                return "FMMC_PR_CABSCHT";
            }
            if (iParam0 == Game.GenerateHash("prop_mb_cargo_03a"))
            {
                return "FMMC_PR_CRG01";
            }
            if (iParam0 == Game.GenerateHash("prop_mb_cargo_04a"))
            {
                return "FMMC_PR_CRG02";
            }
            if (iParam0 == Game.GenerateHash("prop_air_cargo_04a"))
            {
                return "FMMC_PR_CRG03";
            }
            if (iParam0 == Game.GenerateHash("prop_mb_crate_01b"))
            {
                return "FMMC_PR_CRG04";
            }
            if (iParam0 == Game.GenerateHash("prop_air_cargo_01a"))
            {
                return "FMMC_PR_CRGAIR";
            }
            if (iParam0 == Game.GenerateHash("prop_mb_cargo_04b"))
            {
                return "FMMC_PR_CRG05";
            }
            if (iParam0 == Game.GenerateHash("prop_mb_cargo_02a"))
            {
                return "FMMC_PR_CRG06";
            }
            if (iParam0 == 2129093333)
            {
                return "FMMC_PR_SYSTEM::SINAPTX";
            }
            if (iParam0 == Game.GenerateHash("prop_air_stair_01"))
            {
                return "FMMC_PR_STRFLY";
            }
            if (iParam0 == -2103481739)
            {
                return "FMMC_PR_STRRSD";
            }
            if (iParam0 == -68540493)
            {
                return "FMMC_PR_STRLWR";
            }
            if (iParam0 == Game.GenerateHash("prop_air_bagloader"))
            {
                return "FMMC_PR_BAGLDL";
            }
            if (iParam0 == 1683244033)
            {
                return "FMMC_PR_BAGLDH";
            }
            if (iParam0 == 1566353027)
            {
                return "FMMC_PR_PLNTGP";
            }
            if (iParam0 == 11906616)
            {
                return "FMMC_PR_BUSHLD";
            }
            if (iParam0 == -1705943745)
            {
                return "FMMC_PR_BUSHM";
            }
            if (iParam0 == -1656246279)
            {
                return "FMMC_PR_BUSHS";
            }
            if (iParam0 == 1052756483)
            {
                return "FMMC_PR_JOTREE";
            }
            if (iParam0 == -492137526)
            {
                return "FMMC_PR_CACTUS";
            }
            if (iParam0 == -593160806)
            {
                return "FMMC_PR_TREFLN";
            }
            if (iParam0 == -503017940)
            {
                return "FMMC_PR_PLNTAP";
            }
            if (iParam0 == 702880916)
            {
                return "FMMC_PR_PLNTPP";
            }
            if (iParam0 == -1203783005)
            {
                return "FMMC_PR_PLNTFW";
            }
            if (iParam0 == 1435537088)
            {
                return "FMMC_PR_PLNTCF";
            }
            if (iParam0 == 933214217)
            {
                return "FMMC_PR_PLNTTC";
            }
            if (iParam0 == 2042668880)
            {
                return "FMMC_PR_RCKBGR";
            }
            if (iParam0 == 390804950)
            {
                return "FMMC_PR_RCKMDF";
            }
            if (iParam0 == 725387438)
            {
                return "FMMC_PR_RCKBGF";
            }
            if (iParam0 == 725387438)
            {
                return "FMMC_PR_RCKBGF";
            }
            if (iParam0 == 145818549)
            {
                return "FMMC_PR_WKLGHT1a";
            }
            if (iParam0 == 160663734)
            {
                return "FMMC_PR_WKLGHT2a";
            }
            if (iParam0 == -350795026)
            {
                return "FMMC_PR_WKLGHT3a";
            }
            if (iParam0 == -1901227524)
            {
                return "FMMC_PR_WKLGHT3b";
            }
            if (iParam0 == 1813008354)
            {
                return "FMMC_PR_WKLGHT4b";
            }
            if (iParam0 == 279288106)
            {
                return "FMMC_PR_WKLGHT4d";
            }
            if (iParam0 == Game.GenerateHash("prop_ind_coalcar_02"))
            {
                return "FMMC_PR_COALCAR";
            }
            if (iParam0 == Game.GenerateHash("prop_crashed_heli"))
            {
                return "FMMC_PR_CRSHHELI";
            }
            if (iParam0 == Game.GenerateHash("prop_water_ramp_01"))
            {
                return "FMMC_PR_WTRRAMP1";
            }
            if (iParam0 == Game.GenerateHash("prop_water_ramp_02"))
            {
                return "FMMC_PR_WTRRAMP2";
            }
            if (iParam0 == Game.GenerateHash("prop_water_ramp_03"))
            {
                return "FMMC_PR_WTRRAMP3";
            }
            if (iParam0 == Game.GenerateHash("prop_offroad_barrel01"))
            {
                return "FMMC_DPR_BARREL";
            }
            if (iParam0 == -996988174)
            {
                return "FMMC_DPR_BRLLNE";
            }
            if (iParam0 == -1344435013)
            {
                return "FMMC_DPR_EXPBRL";
            }
            if (iParam0 == -1980225301)
            {
                return "FMMC_DPR_FIREXT";
            }
            if (iParam0 == Game.GenerateHash("prop_roadcone02c"))
            {
                return "FMMC_DPR_CONE";
            }
            if (iParam0 == Game.GenerateHash("prop_roadcone02a"))
            {
                return "FMMC_DPR_TRFCNE";
            }
            if (iParam0 == Game.GenerateHash("prop_roadcone01a"))
            {
                return "FMMC_DPR_LTRFCN";
            }
            if (iParam0 == 10928689)
            {
                return "FMMC_DPR_TRFPLE";
            }
            if (iParam0 == 1363150739)
            {
                return "FMMC_DPR_MALBOX";
            }
            if (iParam0 == 917457845)
            {
                return "FMMC_DPR_NPVND";
            }
            if (iParam0 == 1099892058)
            {
                return "FMMC_DPR_WVND";
            }
            if (iParam0 == -1034034125)
            {
                return "FMMC_DPR_MCHSNK";
            }
            if (iParam0 == -455396574)
            {
                return "FMMC_DPR_MCHTCK";
            }
            if (iParam0 == 1524671283)
            {
                return "FMMC_DPR_BOXPIL";
            }
            if (iParam0 == 2139919312)
            {
                return "FMMC_DPR_DESBAR";
            }
            if (iParam0 == -1669382392)
            {
                return "FMMC_DPR_SECFEN";
            }
            if (iParam0 == Game.GenerateHash("prop_table_08_side"))
            {
                return "FMMC_DPR_UPTTBL";
            }
            if (iParam0 == -699955605)
            {
                return "FMMC_DPR_AMOCRT";
            }
            if (iParam0 == 1246158990)
            {
                return "FMMC_DPR_ORDNAN";
            }
            if (iParam0 == 1890640474)
            {
                return "FMMC_DPR_TKRDEX";
            }
            if (iParam0 == -37176073)
            {
                return "FMMC_DPR_LGPLEL";
            }
            if (iParam0 == -1381557071)
            {
                return "FMMC_DPR_LGPLES";
            }
            if (iParam0 == 1530421247)
            {
                return "FMMC_DPR_PIPPLE";
            }
            if (iParam0 == 1652026494)
            {
                return "FMMC_DPR_BRLPLE";
            }
            if (iParam0 == -1088738506)
            {
                return "FMMC_DPR_BRGOEX";
            }
            if (iParam0 == -1935686084)
            {
                return "FMMC_DPR_BRFLEX";
            }
            if (iParam0 == -46303329)
            {
                return "FMMC_DPR_TKLRGW";
            }
            if (iParam0 == -9837968)
            {
                return "FMMC_DPR_TKLRGG";
            }
            if (iParam0 == -353447166)
            {
                return "FMMC_DPR_TKLRGY";
            }
            if (iParam0 == 786272259)
            {
                return "FMMC_DPR_JRYCAN";
            }
            if (iParam0 == 1270590574)
            {
                return "FMMC_DPR_SGC";
            }
            if (iParam0 == -1029296059)
            {
                return "FMMC_DPR_LGC";
            }
            if (iParam0 == -1918614878)
            {
                return "FMMC_DPR_TBGAS";
            }
            if (iParam0 == 1257553220)
            {
                return "FMMC_DPR_TRGAS";
            }
            if (iParam0 == 2138646444)
            {
                return "FMMC_DPR_OLGAS";
            }
            if (iParam0 == -672016228)
            {
                return "FMMC_DPR_OSGAS";
            }
            if (iParam0 == 858993389)
            {
                return "FMMC_DPR_FSTND";
            }
            if (iParam0 == Game.GenerateHash("prop_rub_tyre_03"))
            {
                return "FMMC_DPR_RBRTYR";
            }
            if (iParam0 == Game.GenerateHash("prop_barrel_02b"))
            {
                return "FMMC_DPR_BAR2";
            }
            if (iParam0 == Game.GenerateHash("prop_barrel_01a"))
            {
                return "FMMC_DPR_BAR3";
            }
            if (iParam0 == -1913970053)
            {
                return "FMMC_DPR_BP3";
            }
            if (iParam0 == 1753330262)
            {
                return "FMMC_DPR_CP1";
            }
            if (iParam0 == -1600413027)
            {
                return "FMMC_DPR_CP2";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_bowling_pin"))
            {
                return "MC_PR_STNT63";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_bowling_ball"))
            {
                return "MC_PR_STNT261";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_soccer_lball"))
            {
                return "MC_PR_STNT65";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_soccer_sball"))
            {
                return "MC_PR_STNT66";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_soccer_ball"))
            {
                return "MC_PR_STNT68";
            }
            if (iParam0 == Game.GenerateHash("prop_ld_alarm_01"))
            {
                return "FMMC_PR_ALARM";
            }
            if (iParam0 == Game.GenerateHash("prop_flare_01"))
            {
                return "FMMC_PR_FLARE";
            }
            if (iParam0 == -1118757580)
            {
                return "FMMC_PR_FIREW";
            }
            if (iParam0 == -143315610)
            {
                return "FMMC_PR_PBARR";
            }
            if (iParam0 == 1572208841)
            {
                return "FMMC_PR_AMFLG";
            }
            if (iParam0 == 301970060)
            {
                return "FMMC_PR_BPLUG";
            }
            if (iParam0 == 1088428993)
            {
                return "FMMC_PR_AMLG1";
            }
            if (iParam0 == -2002895309)
            {
                return "FMMC_PR_AMLG2";
            }
            if (iParam0 == 769923921)
            {
                return "FMMC_PR_CASHT";
            }
            if (iParam0 == -1941093436)
            {
                return "FMMC_PR_CARL2";
            }
            if (iParam0 == 1644490552)
            {
                return "FMMC_PR_CARL5";
            }
            if (iParam0 == 1228163930)
            {
                return "FMMC_PR_WALLL";
            }
            if (iParam0 == -82704061)
            {
                return "FMMC_PR_MNKCR";
            }
            if (iParam0 == -893826075)
            {
                return "FMMC_PR_METCV";
            }
            if (iParam0 == -1906772306)
            {
                return "FMMC_PR_BNKAL";
            }
            if (iParam0 == 1325339411)
            {
                return "FMMC_PR_RDMEM";
            }
            if (iParam0 == Game.GenerateHash("prop_boombox_01"))
            {
                return "FMMC_PR_BBRE";
            }
            if (iParam0 == Game.GenerateHash("prop_ghettoblast_02"))
            {
                return "FMMC_PR_GHBL";
            }
            if (iParam0 == Game.GenerateHash("prop_tapeplayer_01"))
            {
                return "FMMC_PR_TAPL";
            }
            if (iParam0 == 2057223314)
            {
                return "FMMC_PR_RADI";
            }
            if (iParam0 == -1611832715)
            {
                return "FMMC_PR_FRWK";
            }
            if (iParam0 == 1781931203)
            {
                return "FMMC_PR_ARMS";
            }
            if (iParam0 == -817529322)
            {
                return "FMMC_PR_LHB";
            }
            if (iParam0 == -857034963)
            {
                return "FMMC_PR_MHB";
            }
            if (iParam0 == -1389865022)
            {
                return "FMMC_PR_SHB";
            }
            if (iParam0 == 1451442166)
            {
                return "FMMC_PR_XLHB";
            }
            if (iParam0 == 869398406)
            {
                return "FMMC_PR_BJT1";
            }
            if (iParam0 == -170277480)
            {
                return "FMMC_PR_BJT2";
            }
            if (iParam0 == -1267801772)
            {
                return "FMMC_PR_BJT3";
            }
            if (iParam0 == -1876506235)
            {
                return "FMMC_STR_WP_52";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_ramp_adj_flip_m"))
            {
                return "MC_PR_STNT173";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_ramp_adj_flip_mb"))
            {
                return "MC_PR_STNT32";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_ramp_adj_flip_s"))
            {
                return "MC_PR_STNT193";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_ramp_adj_flip_sb"))
            {
                return "MC_PR_STNT31";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_ramp_adj_hloop"))
            {
                return "MC_PR_STNT19";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_ramp_adj_loop"))
            {
                return "MC_PR_STNT20";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_ramp_jump_xs"))
            {
                return "MC_PR_STNT25";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_ramp_jump_s"))
            {
                return "MC_PR_STNT26";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_ramp_jump_m"))
            {
                return "MC_PR_STNT27";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_ramp_jump_l"))
            {
                return "MC_PR_STNT28";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_ramp_jump_xl"))
            {
                return "MC_PR_STNT29";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_ramp_jump_xxl"))
            {
                return "MC_PR_STNT30";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_jump_01a"))
            {
                return "MC_PR_STNT156";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_jump_01b"))
            {
                return "MC_PR_STNT157";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_jump_01c"))
            {
                return "MC_PR_STNT158";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_jump_02a"))
            {
                return "MC_PR_STNT159";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_jump_02b"))
            {
                return "MC_PR_STNT160";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_jump_02c"))
            {
                return "MC_PR_STNT161";
            }
            if (iParam0 == -469285935)
            {
                return "MC_PR_STNT156";
            }
            if (iParam0 == -193600338)
            {
                return "MC_PR_STNT157";
            }
            if (iParam0 == 260774616)
            {
                return "MC_PR_STNT158";
            }
            if (iParam0 == -131993690)
            {
                return "MC_PR_STNT159";
            }
            if (iParam0 == 29885170)
            {
                return "MC_PR_STNT160";
            }
            if (iParam0 == -931000217)
            {
                return "MC_PR_STNT161";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_ramp_multi_loop_rb"))
            {
                return "MC_PR_STNT21";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_jump_loop"))
            {
                return "MC_PR_STNT319";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_ramp_spiral_s"))
            {
                return "MC_PR_STNT24";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_ramp_spiral_l_s"))
            {
                return "MC_PR_STNT150";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_ramp_spiral_m"))
            {
                return "MC_PR_STNT23";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_ramp_spiral_l_m"))
            {
                return "MC_PR_STNT151";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_ramp_spiral_l"))
            {
                return "MC_PR_STNT22";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_ramp_spiral_l_l"))
            {
                return "MC_PR_STNT152";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_ramp_spiral_xxl"))
            {
                return "MC_PR_STNT155";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_ramp_spiral_l_xxl"))
            {
                return "MC_PR_STNT153";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_bowlpin_stand"))
            {
                return "MC_PR_STNT64";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_jump_s"))
            {
                return "MC_PR_STNT88";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_jump_m"))
            {
                return "MC_PR_STNT89";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_jump_l"))
            {
                return "MC_PR_STNT90";
            }
            if (iParam0 == 1522336754)
            {
                return "MC_PR_STNT88";
            }
            if (iParam0 == -1376081264)
            {
                return "MC_PR_STNT89";
            }
            if (iParam0 == 1123013752)
            {
                return "MC_PR_STNT90";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_jump_sb"))
            {
                return "MC_PR_STNT110";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_jump_mb"))
            {
                return "MC_PR_STNT111";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_jump_lb"))
            {
                return "MC_PR_STNT112";
            }
            if (iParam0 == 1009862996)
            {
                return "MC_PR_STNT110";
            }
            if (iParam0 == -1101562359)
            {
                return "MC_PR_STNT111";
            }
            if (iParam0 == 563005858)
            {
                return "MC_PR_STNT112";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_ramp"))
            {
                return "MC_PR_STNT33";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_wideramp"))
            {
                return "MC_PR_STNT135";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_bblock_qp3"))
            {
                return "MC_PR_STNT162";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_bblock_qp2"))
            {
                return "MC_PR_STNT163";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_bblock_qp"))
            {
                return "MC_PR_STNT164";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_bblock_hump_01"))
            {
                return "MC_PR_STNT163s";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_bblock_hump_02"))
            {
                return "MC_PR_STNT163m";
            }
            if (iParam0 == 80379378)
            {
                return "MC_PR_BKRQC";
            }
            if (iParam0 == -1111661991)
            {
                return "MC_PR_BKRQCM";
            }
            if (iParam0 == -1132240923)
            {
                return "MC_PR_BKRQCL";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_soccer_goal"))
            {
                return "MC_PR_STNT67";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_race_start_line_01"))
            {
                return "MC_PR_STNT190";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_race_start_line_01b"))
            {
                return "MC_PR_STNT190b";
            }
            if (iParam0 == -740259979)
            {
                return "MC_PR_STNT191";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_race_start_line_02b"))
            {
                return "MC_PR_STNT243";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_race_start_line_03"))
            {
                return "MC_PR_STNT192";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_race_start_line_03b"))
            {
                return "MC_PR_STNT244";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_target_small"))
            {
                return "MC_PR_STNT320";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_target"))
            {
                return "MC_PR_STNT34";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_landing_zone_01"))
            {
                return "MC_PR_STNT171";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_hoop_tyre_01a"))
            {
                return "MC_PR_STNT194";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_crn"))
            {
                return "MC_PR_STNT46";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_crn2"))
            {
                return "MC_PR_STNT102";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_crn_5d"))
            {
                return "MC_PR_STNT226";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_crn_15d"))
            {
                return "MC_PR_STNT227";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_crn_30d"))
            {
                return "MC_PR_STNT228";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_fork"))
            {
                return "MC_PR_STNT134";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_gap_01"))
            {
                return "MC_PR_STNT165";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_gap_02"))
            {
                return "MC_PR_STNT166";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_gap_03"))
            {
                return "MC_PR_STNT262";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_cross"))
            {
                return "MC_PR_STNT40";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_end"))
            {
                return "MC_PR_STNT47";
            }
            if (iParam0 == 658053636)
            {
                return "MC_PR_STNT47b";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_speed"))
            {
                return "MC_PR_STNT248";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_tube_02"))
            {
                return "MC_PR_STNT170";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_qg"))
            {
                return "MC_PR_STNT41";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_hg"))
            {
                return "MC_PR_STNT42";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_xxs"))
            {
                return "MC_PR_STNT104";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_xs"))
            {
                return "MC_PR_STNT37";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_s"))
            {
                return "MC_PR_STNT38";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_m"))
            {
                return "MC_PR_STNT39";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_l"))
            {
                return "MC_PR_STNT100";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_jmp"))
            {
                return "MC_PR_STNT44";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_jmp2"))
            {
                return "MC_PR_STNT82";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_ent"))
            {
                return "MC_PR_STNT93";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_fn_01"))
            {
                return "MC_PR_STNT83";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_fn_02"))
            {
                return "MC_PR_STNT84";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_fn_03"))
            {
                return "MC_PR_STNT85";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_fn_04"))
            {
                return "MC_PR_STNT86";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_tube_fn_05"))
            {
                return "MC_PR_STNT87";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_stop_sign"))
            {
                return "MC_PR_STNT101";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_corner_sign_01"))
            {
                return "MC_PR_STNT174";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_corner_sign_02"))
            {
                return "MC_PR_STNT175";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_corner_sign_03"))
            {
                return "MC_PR_STNT176";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_corner_sign_04"))
            {
                return "MC_PR_STNT177";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_corner_sign_05"))
            {
                return "MC_PR_STNT178";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_corner_sign_06"))
            {
                return "MC_PR_STNT179";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_corner_sign_07"))
            {
                return "MC_PR_STNT180";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_corner_sign_08"))
            {
                return "MC_PR_STNT181";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_corner_sign_09"))
            {
                return "MC_PR_STNT182";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_corner_sign_10"))
            {
                return "MC_PR_STNT183";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_corner_sign_11"))
            {
                return "MC_PR_STNT184";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_corner_sign_12"))
            {
                return "MC_PR_STNT185";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_corner_sign_13"))
            {
                return "MC_PR_STNT186";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_corner_sign_14"))
            {
                return "MC_PR_STNT187";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_sign_circuit_01"))
            {
                return "MC_PR_STNT198";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_sign_circuit_02"))
            {
                return "MC_PR_STNT199";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_sign_circuit_03"))
            {
                return "MC_PR_STNT200";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_sign_circuit_04"))
            {
                return "MC_PR_STNT201";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_sign_circuit_05"))
            {
                return "MC_PR_STNT202";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_sign_circuit_06"))
            {
                return "MC_PR_STNT203";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_sign_circuit_07"))
            {
                return "MC_PR_STNT204";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_sign_circuit_08"))
            {
                return "MC_PR_STNT205";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_sign_circuit_09"))
            {
                return "MC_PR_STNT206";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_sign_circuit_10"))
            {
                return "MC_PR_STNT207";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_sign_circuit_11"))
            {
                return "MC_PR_STNT208";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_sign_circuit_12"))
            {
                return "MC_PR_STNT209";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_sign_circuit_13"))
            {
                return "MC_PR_STNT210";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_sign_circuit_14"))
            {
                return "MC_PR_STNT211";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_sign_circuit_15"))
            {
                return "MC_PR_STNT212";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_bend_bar_m"))
            {
                return "MC_PR_STNT52";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_bend_5d_bar"))
            {
                return "MC_PR_STNT264";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_bend_15d_bar"))
            {
                return "MC_PR_STNT265";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_bend_30d_bar"))
            {
                return "MC_PR_STNT266";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_bend_180d_bar"))
            {
                return "MC_PR_STNT267";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_bend_m"))
            {
                return "MC_PR_STNT48";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_bend_5d"))
            {
                return "MC_PR_STNT222";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_bend_15d"))
            {
                return "MC_PR_STNT223";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_bend_30d"))
            {
                return "MC_PR_STNT224";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_bend_180d"))
            {
                return "MC_PR_STNT225";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_fork"))
            {
                return "MC_PR_STNT137";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_cross"))
            {
                return "MC_PR_STNT56";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_fork_bar"))
            {
                return "MC_PR_STNT138";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_cross_bar"))
            {
                return "MC_PR_STNT57";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_chicane_l"))
            {
                return "MC_PR_STNT167";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_tube_01"))
            {
                return "MC_PR_STNT169";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_chicane_r"))
            {
                return "MC_PR_STNT168";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_chicane_l_02"))
            {
                return "MC_PR_STNT196";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_chicane_r_02"))
            {
                return "MC_PR_STNT197";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_block_01"))
            {
                return "MC_PR_STNT236";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_block_02"))
            {
                return "MC_PR_STNT237";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_block_03"))
            {
                return "MC_PR_STNT238";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_bend_l"))
            {
                return "MC_PR_STNT239";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_bend2_l"))
            {
                return "MC_PR_STNT240";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_bend_l_b"))
            {
                return "MC_PR_STNT49";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_bend2_l_b"))
            {
                return "MC_PR_STNT141";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_bend_bar_l"))
            {
                return "MC_PR_STNT241";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_bend2_bar_l"))
            {
                return "MC_PR_STNT259";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_bend_bar_l_b"))
            {
                return "MC_PR_STNT53";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_bend2_bar_l_b"))
            {
                return "MC_PR_STNT142";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_race_gantry_01"))
            {
                return "MC_PR_STNT242";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_funnel"))
            {
                return "MC_PR_STNT58";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_funnel_ads_01a"))
            {
                return "MC_PR_STNT235";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_funnel_ads_01b"))
            {
                return "MC_PR_STNT253";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_funnel_ads_01c"))
            {
                return "MC_PR_STNT254";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_link"))
            {
                return "MC_PR_STNT115";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_start"))
            {
                return "MC_PR_STNT59";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_start_02"))
            {
                return "MC_PR_STNT2";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_straight_bar_l"))
            {
                return "MC_PR_STNT55";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_straight_lm_bar"))
            {
                return "MC_PR_STNT55a";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_straight_bar_m"))
            {
                return "MC_PR_STNT54";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_straight_bar_s"))
            {
                return "MC_PR_STNT106";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_straight_l"))
            {
                return "MC_PR_STNT51";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_straight_lm"))
            {
                return "MC_PR_STNT51a";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_straight_m"))
            {
                return "MC_PR_STNT50";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_straight_s"))
            {
                return "MC_PR_STNT105";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_wallride_01"))
            {
                return "MC_PR_STNT256";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_wallride_01b"))
            {
                return "MC_PR_STNT91";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_wallride_04"))
            {
                return "MC_PR_STNT60";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_wallride_45r"))
            {
                return "MC_PR_STNT143";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_wallride_45ra"))
            {
                return "MC_PR_STNT249";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_wallride_45l"))
            {
                return "MC_PR_STNT144";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_wallride_45la"))
            {
                return "MC_PR_STNT250";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_wallride_90r"))
            {
                return "MC_PR_STNT145";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_wallride_90rb"))
            {
                return "MC_PR_STNT251";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_wallride_90l"))
            {
                return "MC_PR_STNT146";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_wallride_90lb"))
            {
                return "MC_PR_STNT252";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_wallride_02"))
            {
                return "MC_PR_STNT257";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_wallride_02b"))
            {
                return "MC_PR_STNT92";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_wallride_05"))
            {
                return "MC_PR_STNT258";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_wallride_05b"))
            {
                return "MC_PR_STNT61";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_exshort"))
            {
                return "MC_PR_STNT154";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_short"))
            {
                return "MC_PR_STNT103";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_straight"))
            {
                return "MC_PR_STNT80";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_cutout"))
            {
                return "MC_PR_STNT113";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_otake"))
            {
                return "MC_PR_STNT69";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_fork"))
            {
                return "MC_PR_STNT139";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_funnel"))
            {
                return "MC_PR_STNT70";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_funlng"))
            {
                return "MC_PR_STNT140";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_slope15"))
            {
                return "MC_PR_STNT74";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_slope30"))
            {
                return "MC_PR_STNT75";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_slope45"))
            {
                return "MC_PR_STNT76";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_hill"))
            {
                return "MC_PR_STNT77";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_hill2"))
            {
                return "MC_PR_STNT78";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_bumps"))
            {
                return "MC_PR_STNT136";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_jump"))
            {
                return "MC_PR_STNT79";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_jump15"))
            {
                return "MC_PR_STNT116";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_jump30"))
            {
                return "MC_PR_STNT117";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_jump45"))
            {
                return "MC_PR_STNT118";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_start"))
            {
                return "MC_PR_STNT71";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_start_02"))
            {
                return "MC_PR_STNT1";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_st_01"))
            {
                return "MC_PR_STNT246";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_st_02"))
            {
                return "MC_PR_STNT245";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_turn"))
            {
                return "MC_PR_STNT73";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_sh15"))
            {
                return "MC_PR_STNT107";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_sh30"))
            {
                return "MC_PR_STNT108";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_sh45"))
            {
                return "MC_PR_STNT109";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_sh45_a"))
            {
                return "MC_PR_STNT109A";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_uturn"))
            {
                return "MC_PR_STNT81";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_link"))
            {
                return "MC_PR_STNT114";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_dwlink"))
            {
                return "MC_PR_STNT3";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_dwlink_02"))
            {
                return "MC_PR_STNT247";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_dwshort"))
            {
                return "MC_PR_STNT4";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_dwsh15"))
            {
                return "MC_PR_STNT5";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_dwturn"))
            {
                return "MC_PR_STNT6";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_dwuturn"))
            {
                return "MC_PR_STNT7";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_dwslope15"))
            {
                return "MC_PR_STNT8";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_dwslope30"))
            {
                return "MC_PR_STNT9";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_track_dwslope45"))
            {
                return "MC_PR_STNT10";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_speedup"))
            {
                return "MC_PR_STNT132";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_speedup_t1"))
            {
                return "MC_PR_STNT231";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_speedup_t2"))
            {
                return "MC_PR_STNT232";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_slowdown"))
            {
                return "MC_PR_STNT133";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_slowdown_t1"))
            {
                return "MC_PR_STNT233";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_track_slowdown_t2"))
            {
                return "MC_PR_STNT234";
            }
            if (iParam0 == -583990942)
            {
                return "MC_PR_STNT6";
            }
            if (iParam0 == 1601693814)
            {
                return "MC_PR_STNT7";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_bblock_sml1"))
            {
                return "MC_PR_STNT120";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_bblock_sml2"))
            {
                return "MC_PR_STNT121";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_bblock_sml3"))
            {
                return "MC_PR_STNT122";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_bblock_mdm1"))
            {
                return "MC_PR_STNT123";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_bblock_mdm2"))
            {
                return "MC_PR_STNT124";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_bblock_mdm3"))
            {
                return "MC_PR_STNT125";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_bblock_lrg1"))
            {
                return "MC_PR_STNT126";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_bblock_lrg2"))
            {
                return "MC_PR_STNT127";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_bblock_lrg3"))
            {
                return "MC_PR_STNT128";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_bblock_xl1"))
            {
                return "MC_PR_STNT129";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_bblock_xl2"))
            {
                return "MC_PR_STNT130";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_bblock_xl3"))
            {
                return "MC_PR_STNT131";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_bblock_huge_01"))
            {
                return "MC_PR_STNT147";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_bblock_huge_02"))
            {
                return "MC_PR_STNT148";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_bblock_huge_03"))
            {
                return "MC_PR_STNT149";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_bblock_huge_04"))
            {
                return "MC_PR_STNT229";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_stunt_bblock_huge_05"))
            {
                return "MC_PR_STNT230";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_hoop_small_01"))
            {
                return "MC_PR_STNT188";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_hoop_constraction_01a"))
            {
                return "MC_PR_STNT189";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_race_tannoy"))
            {
                return "MC_PR_STNT255";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_01"))
            {
                return "MC_PR_STNT213";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_02"))
            {
                return "MC_PR_STNT214";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_03"))
            {
                return "MC_PR_STNT215";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_04"))
            {
                return "MC_PR_STNT274";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_05"))
            {
                return "MC_PR_STNT275";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_06"))
            {
                return "MC_PR_STNT276";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_07"))
            {
                return "MC_PR_STNT277";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_08"))
            {
                return "MC_PR_STNT278";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_09"))
            {
                return "MC_PR_STNT279";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_010"))
            {
                return "MC_PR_STNT280";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_011"))
            {
                return "MC_PR_STNT281";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_012"))
            {
                return "MC_PR_STNT282";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_013"))
            {
                return "MC_PR_STNT283";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_014"))
            {
                return "MC_PR_STNT316";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_015"))
            {
                return "MC_PR_STNT284";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0r1"))
            {
                return "MC_PR_STNT216";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0r2"))
            {
                return "MC_PR_STNT217";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0r06"))
            {
                return "MC_PR_STNT285";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0r07"))
            {
                return "MC_PR_STNT286";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0r011"))
            {
                return "MC_PR_STNT287";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0r012"))
            {
                return "MC_PR_STNT288";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0r013"))
            {
                return "MC_PR_STNT289";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0r014"))
            {
                return "MC_PR_STNT290";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0r019"))
            {
                return "MC_PR_STNT291";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0r3"))
            {
                return "MC_PR_STNT218";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0r04"))
            {
                return "MC_PR_STNT292";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0r05"))
            {
                return "MC_PR_STNT293";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0r08"))
            {
                return "MC_PR_STNT294";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0r09"))
            {
                return "MC_PR_STNT295";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0r010"))
            {
                return "MC_PR_STNT296";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0r015"))
            {
                return "MC_PR_STNT297";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0r016"))
            {
                return "MC_PR_STNT298";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0r017"))
            {
                return "MC_PR_STNT299";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0r018"))
            {
                return "MC_PR_STNT300";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0l1"))
            {
                return "MC_PR_STNT219";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0l2"))
            {
                return "MC_PR_STNT220";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0l06"))
            {
                return "MC_PR_STNT301";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0l07"))
            {
                return "MC_PR_STNT302";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0l013"))
            {
                return "MC_PR_STNT303";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0l014"))
            {
                return "MC_PR_STNT304";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0l015"))
            {
                return "MC_PR_STNT305";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0l020"))
            {
                return "MC_PR_STNT306";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0l3"))
            {
                return "MC_PR_STNT221";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0l04"))
            {
                return "MC_PR_STNT307";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0l05"))
            {
                return "MC_PR_STNT308";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0l08"))
            {
                return "MC_PR_STNT309";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0l010"))
            {
                return "MC_PR_STNT310";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0l012"))
            {
                return "MC_PR_STNT311";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0l16"))
            {
                return "MC_PR_STNT312";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0l17"))
            {
                return "MC_PR_STNT313";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0l018"))
            {
                return "MC_PR_STNT314";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_tyre_wall_0l019"))
            {
                return "MC_PR_STNT315";
            }
            if (iParam0 == Game.GenerateHash("stt_prop_speakerstack_01a"))
            {
                return "MC_PR_STNT263";
            }
            if (iParam0 == 1696520608)
            {
                return "MC_BKR_DG_0";
            }
            if (iParam0 == -1026035001)
            {
                return "MC_BKR_DG_1";
            }
            if (iParam0 == 1002246134)
            {
                return "MC_BKR_DG_2";
            }
            if (iParam0 == -2122380018)
            {
                return "MC_BKR_DG_3";
            }
            if (iParam0 == Game.GenerateHash("prop_keg_01"))
            {
                return "MC_BKR_DG_4";
            }
            if (iParam0 == -515655246)
            {
                return "MC_BKR_DG_5";
            }
            if (iParam0 == 538990259)
            {
                return "MC_BKR_DG_6";
            }
            if (iParam0 == -524235091)
            {
                return "MC_BKR_DG_7";
            }
            if (iParam0 == -2059889071)
            {
                return "MC_BKR_DG_8";
            }
            if (iParam0 == 652625140)
            {
                return "MC_BKR_DG_9";
            }
            if (iParam0 == 518749770)
            {
                return "MC_BKR_DG_10";
            }
            if (iParam0 == -1570688006)
            {
                return "MC_BKR_DG_11";
            }
            if (iParam0 == -1902227152)
            {
                return "MC_BKR_DG_13";
            }
            if (iParam0 == 469652573)
            {
                return "MC_BKR_DG_14";
            }
            if (iParam0 == 716763602)
            {
                return "MC_BKR_DG_15";
            }
            if (iParam0 == -928937343)
            {
                return "MC_BKR_DG_16";
            }
            if (iParam0 == 870605061)
            {
                return "MC_BKR_DG_17";
            }
            if (iParam0 == 1027382312)
            {
                return "MC_BKR_DG_18";
            }
            if (iParam0 == 723401723)
            {
                return "MC_BKR_DG_19";
            }
            if (iParam0 == -630267126)
            {
                return "MC_BKR_DG_20";
            }
            if (iParam0 == -1344401943)
            {
                return "MC_BKR_DG_21";
            }
            if (iParam0 == -1137432939)
            {
                return "MC_BKR_DG_22";
            }
            if (iParam0 == -1751978709)
            {
                return "MC_BKR_DG_23";
            }
            if (iParam0 == -1524398004)
            {
                return "MC_BKR_DG_24";
            }
            if (iParam0 == 2032545874)
            {
                return "MC_BKR_DG_25";
            }
            if (iParam0 == 552023728)
            {
                return "MC_BKR_DG_26";
            }
            if (iParam0 == 312351262)
            {
                return "MC_BKR_DG_27";
            }
            if (iParam0 == 1165295563)
            {
                return "MC_BKR_DG_28";
            }
            if (iParam0 == 1939128168)
            {
                return "MC_BKR_DG_29";
            }
            if (iParam0 == -2058034456)
            {
                return "MC_BKR_DG_30";
            }
            if (iParam0 == -1775238062)
            {
                return "MC_BKR_DG_31";
            }
            if (iParam0 == -411908812)
            {
                return "MC_BKR_DG_32";
            }
            if (iParam0 == 959604918)
            {
                return "MC_BKR_DG_33";
            }
            if (iParam0 == -875950621)
            {
                return "MC_BKR_DG_34";
            }
            if (iParam0 == 511619919)
            {
                return "MC_BKR_DG_35";
            }
            if (iParam0 == 690735273)
            {
                return "MC_BKR_DG_36";
            }
            if (iParam0 == 2019514094)
            {
                return "MC_BKR_DG_37";
            }
            if (iParam0 == 1490006141)
            {
                return "MC_BKR_DG_38";
            }
            if (iParam0 == -29366490)
            {
                return "MC_BKR_DG_39";
            }
            if (iParam0 == -1460937400)
            {
                return "MC_BKR_DG_40";
            }
            if (iParam0 == 2131364229)
            {
                return "MC_BKR_DG_41";
            }
            if (iParam0 == 1906833154)
            {
                return "MC_IE_PROP_01";
            }
            if (iParam0 == 949515150)
            {
                return "MC_BKR_DG_42";
            }
            if (iParam0 == -109574202)
            {
                return "MC_BKR_DG_43";
            }
            if (iParam0 == -1342281820)
            {
                return "MC_BKR_DG_44";
            }
            if (iParam0 == 1112939169)
            {
                return "MC_BKR_DG_45";
            }
            if (iParam0 == 1178659324)
            {
                return "MC_SR_PROP_01";
            }
            if (iParam0 == 1131265698)
            {
                return "MC_SR_PROP_01";
            }
            if (iParam0 == -1478491595)
            {
                return "MC_SR_PROP_01";
            }
            if (iParam0 == -1566312879)
            {
                return "MC_SR_PROP_01";
            }
            if (iParam0 == 1092990902)
            {
                return "MC_SR_PROP_01";
            }
            if (iParam0 == 731900556)
            {
                return "MC_SR_PROP_02";
            }
            if (iParam0 == -1516020383)
            {
                return "MC_SR_PROP_02";
            }
            if (iParam0 == 1803783398)
            {
                return "MC_SR_PROP_02";
            }
            if (iParam0 == 86720275)
            {
                return "MC_SR_PROP_02";
            }
            if (iParam0 == -781265297)
            {
                return "MC_SR_PROP_02";
            }
            if (iParam0 == 2007085106)
            {
                return "MC_SR_PROP_03";
            }
            if (iParam0 == 1971104468)
            {
                return "MC_SR_PROP_03";
            }
            if (iParam0 == -1029913637)
            {
                return "MC_SR_PROP_03";
            }
            if (iParam0 == -880061280)
            {
                return "MC_SR_PROP_03";
            }
            if (iParam0 == 1429954114)
            {
                return "MC_SR_PROP_03";
            }
            if (iParam0 == -2017664345)
            {
                return "MC_SR_PROP_04";
            }
            if (iParam0 == -1769468195)
            {
                return "MC_SR_PROP_04";
            }
            if (iParam0 == 402624186)
            {
                return "MC_SR_PROP_04";
            }
            if (iParam0 == 33644338)
            {
                return "MC_SR_PROP_04";
            }
            if (iParam0 == -1525241126)
            {
                return "MC_SR_PROP_04";
            }
            if (iParam0 == 1050974173)
            {
                return "MC_SR_PROP_05";
            }
            if (iParam0 == 1387716817)
            {
                return "MC_SR_PROP_05";
            }
            if (iParam0 == 1083095905)
            {
                return "MC_SR_PROP_05";
            }
            if (iParam0 == -1336501237)
            {
                return "MC_SR_PROP_05";
            }
            if (iParam0 == 74400531)
            {
                return "MC_SR_PROP_05";
            }
            if (iParam0 == 2126974554)
            {
                return "MC_SR_PROP_06";
            }
            if (iParam0 == -2009062792)
            {
                return "MC_SR_PROP_06";
            }
            if (iParam0 == -1800160225)
            {
                return "MC_SR_PROP_06";
            }
            if (iParam0 == -1595746923)
            {
                return "MC_SR_PROP_06";
            }
            if (iParam0 == -422679657)
            {
                return "MC_SR_PROP_06";
            }
            if (iParam0 == -265653917)
            {
                return "MC_SR_PROP_07";
            }
            if (iParam0 == -2048411853)
            {
                return "MC_SR_PROP_07";
            }
            if (iParam0 == -1632639189)
            {
                return "MC_SR_PROP_07";
            }
            if (iParam0 == 1832979898)
            {
                return "MC_SR_PROP_07";
            }
            if (iParam0 == -399376234)
            {
                return "MC_SR_PROP_07";
            }
            if (iParam0 == -1131233628)
            {
                return "MC_SR_PROP_08";
            }
            if (iParam0 == -1681425082)
            {
                return "MC_SR_PROP_08";
            }
            if (iParam0 == 1145525423)
            {
                return "MC_SR_PROP_08";
            }
            if (iParam0 == -397344560)
            {
                return "MC_SR_PROP_08";
            }
            if (iParam0 == -1002031213)
            {
                return "MC_SR_PROP_08";
            }
            if (iParam0 == -2027381957)
            {
                return "MC_SR_PROP_09";
            }
            if (iParam0 == -680183673)
            {
                return "MC_SR_PROP_09";
            }
            if (iParam0 == -465809747)
            {
                return "MC_SR_PROP_09";
            }
            if (iParam0 == -450508141)
            {
                return "MC_SR_PROP_09";
            }
            if (iParam0 == -1380106357)
            {
                return "MC_SR_PROP_09";
            }
            if (iParam0 == -1656485084)
            {
                return "MC_SR_PROP_10";
            }
            if (iParam0 == 1894953950)
            {
                return "MC_SR_PROP_10";
            }
            if (iParam0 == -1958188711)
            {
                return "MC_SR_PROP_10";
            }
            if (iParam0 == 685123030)
            {
                return "MC_SR_PROP_10";
            }
            if (iParam0 == 226009766)
            {
                return "MC_SR_PROP_10";
            }
            if (iParam0 == 346118110)
            {
                return "MC_SR_PROP_31";
            }
            if (iParam0 == 1486974596)
            {
                return "MC_SR_PROP_32";
            }
            if (iParam0 == -863549590)
            {
                return "MC_SR_PROP_33";
            }
            if (iParam0 == -1202280195)
            {
                return "MC_SR_PROP_34";
            }
            if (iParam0 == -1834664202)
            {
                return "MC_SR_PROP_35";
            }
            if (iParam0 == -763509440)
            {
                return "MC_SR_PROP_36";
            }
            if (iParam0 == -2065716143)
            {
                return "MC_SR_PROP_37";
            }
            if (iParam0 == -921193496)
            {
                return "MC_SR_PROP_38";
            }
            if (iParam0 == 1374151925)
            {
                return "MC_SR_PROP_39";
            }
            if (iParam0 == 1106139394)
            {
                return "MC_SR_PROP_40";
            }
            if (iParam0 == 1000426600)
            {
                return "MC_SR_PROP_41";
            }
            if (iParam0 == -731135591)
            {
                return "MC_SR_PROP_42";
            }
            if (iParam0 == 1445035920)
            {
                return "MC_SR_PROP_44";
            }
            if (iParam0 == 1575467428)
            {
                return "MC_SR_PROP_45";
            }
            if (iParam0 == 472547144)
            {
                return "MC_SR_PROP_46";
            }
            if (iParam0 == 362681026)
            {
                return "MC_SR_PROP_47";
            }
            if (iParam0 == -1398962413)
            {
                return "MC_SR_PROP_48";
            }
            if (iParam0 == -877963371)
            {
                return "MC_GR_BNKR_DR";
            }
            if (iParam0 == -291659998)
            {
                return "MC_SR_PROP_49";
            }
            if (iParam0 == 1673929480)
            {
                return "MC_SR_PROP_50";
            }
            if (iParam0 == 1708511539)
            {
                return "MC_SR_PROP_51";
            }
            if (iParam0 == -117245244)
            {
                return "MC_SR_PROP_52";
            }
            if (iParam0 == -60015937)
            {
                return "MC_SR_PROP_53";
            }
            if (iParam0 == -1576755324)
            {
                return "MC_SR_PROP_54";
            }
            if (iParam0 == -544828600)
            {
                return "MC_SR_PROP_55";
            }
            if (iParam0 == -558053832)
            {
                return "MC_SR_PROP_56";
            }
            if (iParam0 == -796663061)
            {
                return "MC_SR_PROP_57";
            }
            if (iParam0 == -816612831)
            {
                return "MC_SR_PROP_58";
            }
            if (iParam0 == -237619225)
            {
                return "MC_SR_PROP_59";
            }
            if (iParam0 == 1666830192)
            {
                return "MC_SR_PROP_60x";
            }
            if (iParam0 == -838998190)
            {
                return "MC_SR_PROP_60";
            }
            if (iParam0 == 346059280)
            {
                return "MC_SR_PROP_61";
            }
            if (iParam0 == -613679208)
            {
                return "MC_SR_PROP_61b";
            }
            if (iParam0 == 620582592)
            {
                return "MC_SR_PROP_62";
            }
            if (iParam0 == 1464908646)
            {
                return "MC_SR_PROP_62b";
            }
            if (iParam0 == 85342060)
            {
                return "MC_SR_PROP_63";
            }
            if (iParam0 == -215444591)
            {
                return "MC_SR_PROP_63b";
            }
            if (iParam0 == 483832101)
            {
                return "MC_SR_PROP_64";
            }
            if (iParam0 == -176168332)
            {
                return "MC_SR_PROP_64b";
            }
            if (iParam0 == 930976262)
            {
                return "MC_SR_PROP_65";
            }
            if (iParam0 == 1721462849)
            {
                return "MC_SR_PROP_65b";
            }
            if (iParam0 == 384852939)
            {
                return "MC_SR_PROP_66";
            }
            if (iParam0 == 145606470)
            {
                return "MC_SR_PROP_66b";
            }
            if (iParam0 == 1518201148)
            {
                return "MC_SR_PROP_67";
            }
            if (iParam0 == 1677872320)
            {
                return "MC_SR_PROP_68";
            }
            if (iParam0 == 320088805)
            {
                return "MC_SR_PROP_68b";
            }
            if (iParam0 == 950795200)
            {
                return "MC_SR_PROP_72";
            }
            if (iParam0 == 708828172)
            {
                return "MC_SR_PROP_73";
            }
            if (iParam0 == -864804458)
            {
                return "MC_SR_PROP_74";
            }
            if (iParam0 == -1302470386)
            {
                return "MC_SR_PROP_69";
            }
            if (iParam0 == -1260656854)
            {
                return "MC_SR_PROP_75";
            }
            if (iParam0 == -1875404158)
            {
                return "MC_SR_PROP_76";
            }
            if (iParam0 == 117169896)
            {
                return "MC_SR_PROP_70";
            }
            if (iParam0 == -1479958115)
            {
                return "MC_SR_PROP_71";
            }
            if (iParam0 == 970414739)
            {
                return "MC_GR_PROP_01";
            }
            if (iParam0 == -1187930949)
            {
                return "MC_GR_PROP_02";
            }
            if (iParam0 == -278591439)
            {
                return "MC_GR_PROP_03";
            }
            if (iParam0 == -403635899)
            {
                return "MC_GR_PROP_04";
            }
            if (iParam0 == 443999472)
            {
                return "MC_GR_PROP_05";
            }
            if (iParam0 == -840425311)
            {
                return "MC_GR_PROP_06";
            }
            if (iParam0 == -490398359)
            {
                return "MC_GR_PROP_07";
            }
            if (iParam0 == 1171791475)
            {
                return "MC_GR_PROP_08";
            }
            if (iParam0 == -1592077865)
            {
                return "MC_GR_PROP_09";
            }
            if (iParam0 == -1494923144)
            {
                return "MC_GR_PROP_10";
            }
            if (iParam0 == -863733544)
            {
                return "MC_GR_PROP_11";
            }
            if (iParam0 == -955159266)
            {
                return "MC_GR_PROP_12";
            }
            if (iParam0 == -1673979170)
            {
                return "MC_GR_PROP_13";
            }
            if (iParam0 == 34120519)
            {
                return "MC_GR_PROP_14";
            }
            if (iParam0 == Game.GenerateHash("prop_target_ora_purp_01"))
            {
                return "MC_GR_PROP_15";
            }
            if (iParam0 == -1835862541)
            {
                return "MC_GR_PROP_16";
            }
            if (iParam0 == -1604087404)
            {
                return "MC_GR_PROP_17";
            }
            if (iParam0 == -1251067775)
            {
                return "MC_GR_PROP_18";
            }
            if (iParam0 == -2025890780)
            {
                return "MC_GR_PROP_19";
            }
            if (iParam0 == 170995043)
            {
                return "MC_GR_PROP_20";
            }
            if (iParam0 == -126973474)
            {
                return "MC_GR_PROP_21";
            }
            if (iParam0 == 1726113796)
            {
                return "MC_GR_PROP_22";
            }
            if (iParam0 == 890176606)
            {
                return "MC_GR_PROP_23";
            }
            if (iParam0 == 249707472)
            {
                return "MC_GR_PROP_24";
            }
            if (iParam0 == -1737968014)
            {
                return "MC_GR_PROP_25";
            }
            if (iParam0 == -986153641)
            {
                return "MC_GR_PROP_26";
            }
            if (true)
            {
                if (iParam0 == -1495513247)
                {
                    return "MC_DO_STNT46";
                }
                if (iParam0 == -812083680)
                {
                    return "MC_DO_STNT102";
                }
                if (iParam0 == -1951016952)
                {
                    return "MC_DO_STNT40";
                }
                if (iParam0 == 564151899)
                {
                    return "MC_DO_STNT165";
                }
                if (iParam0 == 710268902)
                {
                    return "MC_DO_STNT166";
                }
                if (iParam0 == 1534868018)
                {
                    return "MC_DO_STNT262";
                }
                if (iParam0 == -38230983)
                {
                    return "MC_DO_STNT104";
                }
                if (iParam0 == -46524906)
                {
                    return "MC_DO_STNT37";
                }
                if (iParam0 == 1060884015)
                {
                    return "MC_DO_STNT38";
                }
                if (iParam0 == -794398462)
                {
                    return "MC_DO_STNT39";
                }
                if (iParam0 == -1009985713)
                {
                    return "MC_DO_STNT100";
                }
            }
            else
            {
                if (iParam0 == -1495513247)
                {
                    return "MC_PR_STNT46";
                }
                if (iParam0 == -812083680)
                {
                    return "MC_PR_STNT102";
                }
                if (iParam0 == -1951016952)
                {
                    return "MC_PR_STNT40";
                }
                if (iParam0 == 564151899)
                {
                    return "MC_PR_STNT165";
                }
                if (iParam0 == 710268902)
                {
                    return "MC_PR_STNT166";
                }
                if (iParam0 == 1534868018)
                {
                    return "MC_PR_STNT262";
                }
                if (iParam0 == -38230983)
                {
                    return "MC_PR_STNT104";
                }
                if (iParam0 == -46524906)
                {
                    return "MC_PR_STNT37";
                }
                if (iParam0 == 1060884015)
                {
                    return "MC_PR_STNT38";
                }
                if (iParam0 == -794398462)
                {
                    return "MC_PR_STNT39";
                }
                if (iParam0 == -1009985713)
                {
                    return "MC_PR_STNT100";
                }
            }
            return "";
        }
        


    }
}
