using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using NativeUI;
using System.Drawing;

namespace MapEditor.Menus
{
    class ObjectPlacingMenu : UIMenu
    {
        public event EventHandler<int> NewObjectSelected;
        public event EventHandler<int> NewObjectTextureVariantSelected;

        UIMenuListItem CategoryItem, TypeItem, PropColorItem;
        UIMenuCheckboxItem SnapObjectsItem = new UIMenuCheckboxItem(Game.GetGXTEntry("FMMC_PRP_SNP"), false);
        int currentCategoryIndex = 0;
        int currentTypeIndex = 0;
        int currentColorIndex = 0;
        List<dynamic> categories = ItemDatabase.PropDB2.Keys.ToList<dynamic>();
        public ObjectPlacingMenu() : base("", "Objects", new Point(0, -107))
        {
            var rectangle = new UIResRectangle();
            rectangle.Color = Color.FromArgb(0, 0, 0, 0);
            SetBannerType(rectangle);
            DisableInstructionalButtons(true);
            MouseControlsEnabled = false;
            ResetKey(UIMenu.MenuControls.Back);
            SetKey(UIMenu.MenuControls.Back, Control.FrontendPauseAlternate);

            List<dynamic> localizedCategories = new List<dynamic>();
            foreach (var item in categories)
            {
                if (Game.DoesGXTEntryExist(item))
                {
                    localizedCategories.Add(Game.GetGXTEntry(item));
                }
                else
                {
                    localizedCategories.Add(item);
                }
            }

            CategoryItem = new UIMenuListItem(Game.GetGXTEntry("FMMC_MTYPE2"), localizedCategories, currentCategoryIndex);
            CategoryItem.OnListChanged += (_, index) =>
            {
                currentCategoryIndex = index;
                Rebuild();
            };

            Rebuild();
        }
        private void Rebuild()
        {
            currentTypeIndex = 0;
            currentColorIndex = 0;
            Clear();
            AddItem(CategoryItem);

            string newCategory = (string)categories[currentCategoryIndex];
            List<dynamic> TypeList = ItemDatabase.PropDB2[newCategory].Cast<dynamic>().ToList();
            List<dynamic> LocalizedList = new List<dynamic>();
            foreach (var item in TypeList)
            {
                string localizedName;
                ItemDatabase.PropGxtDB.TryGetValue(item, out localizedName);
                if (localizedName != null && Game.DoesGXTEntryExist(localizedName))
                {
                    LocalizedList.Add(Game.GetGXTEntry(localizedName));
                }
                else
                {
                    LocalizedList.Add(item);
                }

            }
            TypeItem = new UIMenuListItem("Type", LocalizedList, currentTypeIndex);
            AddItem(TypeItem);
            TypeItem.OnListChanged += (_, index) =>
            {
                string categoryName = (string)categories[currentCategoryIndex];
                string objectname = ItemDatabase.PropDB2[categoryName].Cast<dynamic>().ToList()[index];
                currentTypeIndex = index;
                if (newCategory == "FMMC_PLIB_19") // Because R*
                {
                    objectname = objectname.Remove(objectname.Length - 2, 2) + (currentColorIndex + 1) + "a";
                }
                OnNewObjectSelected(objectname);
                OnNewObjectTextureVariantSelected(currentColorIndex);
            };
            if (newCategory == "FMMC_PLIB_19") // Because R*
            {
                List<dynamic> localizedColorList = new List<dynamic>();
                for (int i = 0; i < 5; i++)
                {
                    localizedColorList.Add(Game.GetGXTEntry($"MC_SR_PROP_C{i}"));
                }
                PropColorItem = new UIMenuListItem(Game.GetGXTEntry("MC_SR_PROP_COL"), localizedColorList, 0);
                PropColorItem.OnListChanged += (_, index) =>
                {
                    string categoryName = (string)categories[currentCategoryIndex];
                    var currentobjectname = ItemDatabase.PropDB2[categoryName].Cast<dynamic>().ToList()[currentTypeIndex];
                    currentColorIndex = index;
                    var objectname = currentobjectname.Remove(currentobjectname.Length-2, 2)+(index+1)+"a";
                    OnNewObjectSelected(objectname);
                };
            }
            else
            {
                SetupColorList(TypeList);
            }
            AddItem(PropColorItem);
            //AddItem(SnapObjectsItem);
            OnNewObjectSelected(TypeList[0]);
            RefreshIndex();
        }
        protected virtual void OnNewObjectSelected(string objname)
        {
            int objhash;
            if (Int32.TryParse(objname, out objhash))
            {
                NewObjectSelected?.Invoke(this, objhash);
            }
            else
            {
                NewObjectSelected?.Invoke(this, Game.GenerateHash(objname));
            }
            
        }

        protected virtual void OnNewObjectTextureVariantSelected(int index)
        {
            NewObjectTextureVariantSelected?.Invoke(this, index);
        }

        public string GetSelectedPropModel()
        {
            string categoryName = (string)categories[currentCategoryIndex];
            return ItemDatabase.PropDB2[categoryName].ToList()[currentTypeIndex];
        }
        public int SelectedColorIndex => currentColorIndex;

        private void SetupColorList(List<dynamic> TypeList)
        {
            string[] colorList = GetPropColorList(TypeList[currentTypeIndex]);
            if (colorList.Length >0)
            {
                List<dynamic> localizedColorList = new List<dynamic>();
                foreach (var item in colorList)
                {
                    localizedColorList.Add(Game.GetGXTEntry(item));
                }
                PropColorItem = new UIMenuListItem(Game.GetGXTEntry("FMMC_PRP_CLR"), localizedColorList, 0);
                PropColorItem.OnListChanged += (_, index) =>
                {
                    currentColorIndex = index;
                    OnNewObjectTextureVariantSelected(index);
                };
            }
            else
            {
                List<dynamic> localizedColorList = new List<dynamic> { Game.GetGXTEntry("FMMC_SEL_DEF") };
                PropColorItem = new UIMenuListItem(Game.GetGXTEntry("FMMC_PRP_CLR"), localizedColorList, 0);
                PropColorItem.Enabled = false;
            }
        }

        private string[] GetPropColorList(string objname)
        {
            int objecthash = joaat(objname);
            if (objecthash == joaat("stt_prop_track_start") || objecthash == joaat("stt_prop_track_start_02") || objecthash == joaat("stt_prop_track_straight_s") || objecthash == joaat("stt_prop_track_straight_m") || objecthash == joaat("stt_prop_track_straight_lm") || objecthash == joaat("stt_prop_track_straight_l") || objecthash == joaat("stt_prop_track_bend_m") || objecthash == joaat("stt_prop_track_bend_l") || objecthash == joaat("stt_prop_track_bend2_l") || objecthash == joaat("stt_prop_track_bend_5d") || objecthash == joaat("stt_prop_track_bend_15d") || objecthash == joaat("stt_prop_track_bend_30d") || objecthash == joaat("stt_prop_track_bend_180d") || objecthash == joaat("stt_prop_track_fork") || objecthash == joaat("stt_prop_track_cross") || objecthash == joaat("stt_prop_track_straight_bar_s") || objecthash == joaat("stt_prop_track_straight_bar_m") || objecthash == joaat("stt_prop_track_straight_lm_bar") || objecthash == joaat("stt_prop_track_straight_bar_l") || objecthash == joaat("stt_prop_track_bend_bar_m") || objecthash == joaat("stt_prop_track_bend_bar_l") || objecthash == joaat("stt_prop_track_bend2_bar_l") || objecthash == joaat("stt_prop_track_bend_5d_bar") || objecthash == joaat("stt_prop_track_bend_15d_bar") || objecthash == joaat("stt_prop_track_bend_30d_bar") || objecthash == joaat("stt_prop_track_bend_180d_bar") || objecthash == joaat("stt_prop_track_fork_bar") || objecthash == joaat("stt_prop_track_cross_bar") || objecthash == joaat("stt_prop_track_funnel") || objecthash == joaat("stt_prop_track_funnel_ads_01a") || objecthash == joaat("stt_prop_track_funnel_ads_01b") || objecthash == joaat("stt_prop_track_funnel_ads_01c") || objecthash == joaat("stt_prop_track_link") || objecthash == joaat("stt_prop_track_chicane_l") || objecthash == joaat("stt_prop_track_chicane_r") || objecthash == joaat("stt_prop_track_chicane_l_02") || objecthash == joaat("stt_prop_track_chicane_r_02") || objecthash == joaat("stt_prop_race_start_line_01") || objecthash == joaat("stt_prop_race_start_line_01b") || objecthash == -740259979 || objecthash == joaat("stt_prop_race_start_line_02b") || objecthash == joaat("stt_prop_race_start_line_03") || objecthash == joaat("stt_prop_race_start_line_03b") || objecthash == joaat("stt_prop_track_block_03") || objecthash == 901501250 || objecthash == -297099405 || objecthash == joaat("stt_prop_track_bend_bar_l_b") || objecthash == joaat("stt_prop_track_bend2_bar_l_b") || objecthash == joaat("stt_prop_track_bend_l_b") || objecthash == joaat("stt_prop_track_bend2_l_b") || objecthash == joaat("stt_prop_track_block_01") || objecthash == joaat("stt_prop_track_block_02") || objecthash == 346118110 || objecthash == 1486974596 || objecthash == -863549590 || objecthash == -1202280195 || objecthash == -1834664202 || objecthash == -763509440 || objecthash == -2065716143 || objecthash == -921193496 || objecthash == joaat("stt_prop_stunt_tube_xxs") || objecthash == -38230983 || objecthash == joaat("stt_prop_stunt_tube_xs") || objecthash == -46524906 || objecthash == joaat("stt_prop_stunt_tube_s") || objecthash == 1060884015 || objecthash == joaat("stt_prop_stunt_tube_m") || objecthash == -794398462 || objecthash == joaat("stt_prop_stunt_tube_l") || objecthash == -1009985713 || objecthash == joaat("stt_prop_stunt_tube_crn") || objecthash == -1495513247 || objecthash == joaat("stt_prop_stunt_tube_crn_5d") || objecthash == joaat("stt_prop_stunt_tube_crn_15d") || objecthash == joaat("stt_prop_stunt_tube_crn_30d") || objecthash == joaat("stt_prop_stunt_tube_crn2") || objecthash == -812083680 || objecthash == joaat("stt_prop_stunt_tube_fork") || objecthash == joaat("stt_prop_stunt_tube_cross") || objecthash == -1951016952 || objecthash == joaat("stt_prop_stunt_tube_gap_01") || objecthash == 564151899 || objecthash == joaat("stt_prop_stunt_tube_gap_02") || objecthash == 710268902 || objecthash == joaat("stt_prop_stunt_tube_gap_03") || objecthash == 1534868018 || objecthash == joaat("stt_prop_stunt_tube_qg") || objecthash == joaat("stt_prop_stunt_tube_hg") || objecthash == joaat("stt_prop_stunt_tube_jmp") || objecthash == joaat("stt_prop_stunt_tube_jmp2") || objecthash == joaat("stt_prop_stunt_tube_fn_01") || objecthash == joaat("stt_prop_stunt_tube_fn_02") || objecthash == joaat("stt_prop_stunt_tube_fn_03") || objecthash == joaat("stt_prop_stunt_tube_fn_04") || objecthash == joaat("stt_prop_stunt_tube_fn_05") || objecthash == joaat("stt_prop_stunt_tube_ent") || objecthash == joaat("stt_prop_stunt_tube_end") || objecthash == 658053636 || objecthash == joaat("stt_prop_track_tube_01") || objecthash == joaat("stt_prop_tyre_wall_01") || objecthash == joaat("stt_prop_tyre_wall_02") || objecthash == joaat("stt_prop_tyre_wall_03") || objecthash == joaat("stt_prop_tyre_wall_04") || objecthash == joaat("stt_prop_tyre_wall_05") || objecthash == joaat("stt_prop_tyre_wall_06") || objecthash == joaat("stt_prop_tyre_wall_07") || objecthash == joaat("stt_prop_tyre_wall_08") || objecthash == joaat("stt_prop_tyre_wall_09") || objecthash == joaat("stt_prop_tyre_wall_010") || objecthash == joaat("stt_prop_tyre_wall_011") || objecthash == joaat("stt_prop_tyre_wall_012") || objecthash == joaat("stt_prop_tyre_wall_013") || objecthash == joaat("stt_prop_tyre_wall_014") || objecthash == joaat("stt_prop_tyre_wall_015") || objecthash == joaat("stt_prop_tyre_wall_0r1") || objecthash == joaat("stt_prop_tyre_wall_0r2") || objecthash == joaat("stt_prop_tyre_wall_0r3") || objecthash == joaat("stt_prop_tyre_wall_0r04") || objecthash == joaat("stt_prop_tyre_wall_0r05") || objecthash == joaat("stt_prop_tyre_wall_0r06") || objecthash == joaat("stt_prop_tyre_wall_0r07") || objecthash == joaat("stt_prop_tyre_wall_0r08") || objecthash == joaat("stt_prop_tyre_wall_0r09") || objecthash == joaat("stt_prop_tyre_wall_0r010") || objecthash == joaat("stt_prop_tyre_wall_0r011") || objecthash == joaat("stt_prop_tyre_wall_0r012") || objecthash == joaat("stt_prop_tyre_wall_0r013") || objecthash == joaat("stt_prop_tyre_wall_0r014") || objecthash == joaat("stt_prop_tyre_wall_0r015") || objecthash == joaat("stt_prop_tyre_wall_0r016") || objecthash == joaat("stt_prop_tyre_wall_0r017") || objecthash == joaat("stt_prop_tyre_wall_0r018") || objecthash == joaat("stt_prop_tyre_wall_0r019") || objecthash == joaat("stt_prop_tyre_wall_0l1") || objecthash == joaat("stt_prop_tyre_wall_0l2") || objecthash == joaat("stt_prop_tyre_wall_0l3") || objecthash == joaat("stt_prop_tyre_wall_0l04") || objecthash == joaat("stt_prop_tyre_wall_0l05") || objecthash == joaat("stt_prop_tyre_wall_0l06") || objecthash == joaat("stt_prop_tyre_wall_0l07") || objecthash == joaat("stt_prop_tyre_wall_0l08") || objecthash == 1784795865 || objecthash == joaat("stt_prop_tyre_wall_0l010") || objecthash == 1307595665 || objecthash == joaat("stt_prop_tyre_wall_0l012") || objecthash == joaat("stt_prop_tyre_wall_0l013") || objecthash == joaat("stt_prop_tyre_wall_0l014") || objecthash == joaat("stt_prop_tyre_wall_0l015") || objecthash == joaat("stt_prop_tyre_wall_0l16") || objecthash == joaat("stt_prop_tyre_wall_0l17") || objecthash == joaat("stt_prop_tyre_wall_0l018") || objecthash == joaat("stt_prop_tyre_wall_0l019") || objecthash == joaat("stt_prop_tyre_wall_0l020") || objecthash == joaat("stt_prop_track_stop_sign") || objecthash == joaat("stt_prop_stunt_target_small") || objecthash == joaat("stt_prop_stunt_target") || objecthash == joaat("stt_prop_stunt_bowlpin_stand") || objecthash == joaat("stt_prop_stunt_landing_zone_01") || objecthash == joaat("stt_prop_hoop_tyre_01a") || objecthash == joaat("stt_prop_stunt_bblock_sml1") || objecthash == joaat("stt_prop_stunt_bblock_sml2") || objecthash == joaat("stt_prop_stunt_bblock_sml3") || objecthash == joaat("stt_prop_stunt_bblock_mdm1") || objecthash == joaat("stt_prop_stunt_bblock_mdm2") || objecthash == joaat("stt_prop_stunt_bblock_mdm3") || objecthash == joaat("stt_prop_stunt_bblock_lrg1") || objecthash == joaat("stt_prop_stunt_bblock_lrg2") || objecthash == joaat("stt_prop_stunt_bblock_lrg3") || objecthash == joaat("stt_prop_stunt_bblock_xl1") || objecthash == joaat("stt_prop_stunt_bblock_xl2") || objecthash == joaat("stt_prop_stunt_bblock_xl3") || objecthash == joaat("stt_prop_stunt_bblock_huge_01") || objecthash == joaat("stt_prop_stunt_bblock_huge_02") || objecthash == joaat("stt_prop_stunt_bblock_huge_03") || objecthash == joaat("stt_prop_stunt_bblock_huge_04") || objecthash == joaat("stt_prop_stunt_bblock_huge_05") || objecthash == joaat("stt_prop_stunt_bowling_ball") || objecthash == 1390188982 || objecthash == joaat("stt_prop_stunt_soccer_goal") || objecthash == joaat("stt_prop_stunt_track_start") || objecthash == joaat("stt_prop_stunt_track_start_02") || objecthash == joaat("stt_prop_stunt_track_st_01") || objecthash == joaat("stt_prop_stunt_track_st_02") || objecthash == joaat("stt_prop_stunt_track_exshort") || objecthash == joaat("stt_prop_stunt_track_short") || objecthash == joaat("stt_prop_stunt_track_straight") || objecthash == joaat("stt_prop_stunt_track_turn") || objecthash == joaat("stt_prop_stunt_track_sh15") || objecthash == joaat("stt_prop_stunt_track_sh30") || objecthash == joaat("stt_prop_stunt_track_sh45") || objecthash == joaat("stt_prop_stunt_track_sh45_a") || objecthash == joaat("stt_prop_stunt_track_uturn") || objecthash == joaat("stt_prop_stunt_track_cutout") || objecthash == joaat("stt_prop_stunt_track_otake") || objecthash == joaat("stt_prop_stunt_track_fork") || objecthash == joaat("stt_prop_stunt_track_funnel") || objecthash == joaat("stt_prop_stunt_track_funlng") || objecthash == joaat("stt_prop_stunt_track_slope15") || objecthash == joaat("stt_prop_stunt_track_slope30") || objecthash == joaat("stt_prop_stunt_track_slope45") || objecthash == joaat("stt_prop_stunt_track_link") || objecthash == joaat("stt_prop_stunt_track_dwlink") || objecthash == joaat("stt_prop_stunt_track_dwlink_02") || objecthash == joaat("stt_prop_stunt_track_hill") || objecthash == joaat("stt_prop_stunt_track_hill2") || objecthash == joaat("stt_prop_stunt_track_bumps") || objecthash == joaat("stt_prop_stunt_track_jump") || objecthash == joaat("stt_prop_stunt_jump15") || objecthash == joaat("stt_prop_stunt_jump30") || objecthash == joaat("stt_prop_stunt_jump45") || objecthash == joaat("stt_prop_stunt_track_dwshort") || objecthash == joaat("stt_prop_stunt_track_dwsh15") || objecthash == joaat("stt_prop_stunt_track_dwturn") || objecthash == joaat("stt_prop_stunt_track_dwuturn") || objecthash == joaat("stt_prop_stunt_track_dwslope15") || objecthash == joaat("stt_prop_stunt_track_dwslope30") || objecthash == joaat("stt_prop_stunt_track_dwslope45") || objecthash == joaat("stt_prop_track_tube_02") || objecthash == joaat("stt_prop_stunt_jump_s") || objecthash == joaat("stt_prop_stunt_jump_m") || objecthash == joaat("stt_prop_stunt_jump_l") || objecthash == joaat("stt_prop_stunt_jump_sb") || objecthash == joaat("stt_prop_stunt_jump_mb") || objecthash == joaat("stt_prop_stunt_jump_lb") || objecthash == joaat("stt_prop_ramp_jump_xs") || objecthash == joaat("stt_prop_ramp_jump_s") || objecthash == joaat("stt_prop_ramp_jump_m") || objecthash == joaat("stt_prop_ramp_jump_l") || objecthash == joaat("stt_prop_ramp_jump_xl") || objecthash == joaat("stt_prop_ramp_jump_xxl") || objecthash == joaat("stt_prop_track_jump_01a") || objecthash == joaat("stt_prop_track_jump_01b") || objecthash == joaat("stt_prop_track_jump_01c") || objecthash == joaat("stt_prop_track_jump_02a") || objecthash == joaat("stt_prop_track_jump_02b") || objecthash == joaat("stt_prop_track_jump_02c") || objecthash == joaat("stt_prop_ramp_adj_flip_s") || objecthash == joaat("stt_prop_ramp_adj_flip_sb") || objecthash == joaat("stt_prop_ramp_adj_flip_m") || objecthash == joaat("stt_prop_ramp_adj_flip_mb") || objecthash == joaat("stt_prop_stunt_ramp") || objecthash == joaat("stt_prop_stunt_wideramp") || objecthash == joaat("stt_prop_stunt_bblock_qp") || objecthash == joaat("stt_prop_stunt_bblock_qp2") || objecthash == joaat("stt_prop_stunt_bblock_qp3") || objecthash == joaat("stt_prop_stunt_bblock_hump_01") || objecthash == joaat("stt_prop_stunt_bblock_hump_02") || objecthash == 1522336754 || objecthash == -1376081264 || objecthash == 1123013752 || objecthash == 1009862996 || objecthash == -1101562359 || objecthash == 563005858 || objecthash == -469285935 || objecthash == -193600338 || objecthash == 260774616 || objecthash == -131993690 || objecthash == 29885170 || objecthash == -931000217 || objecthash == joaat("stt_prop_stunt_wideramp") || objecthash == -29366490 || objecthash == 1490006141 || objecthash == 2019514094 || objecthash == -1460937400 || objecthash == 2131364229 || objecthash == 80379378 || objecthash == -1111661991 || objecthash == -1132240923 || objecthash == joaat("stt_prop_wallride_05") || objecthash == joaat("stt_prop_ramp_adj_loop") || objecthash == joaat("stt_prop_ramp_multi_loop_rb") || objecthash == joaat("stt_prop_stunt_jump_loop") || objecthash == joaat("stt_prop_ramp_spiral_s") || objecthash == joaat("stt_prop_ramp_spiral_l_s") || objecthash == joaat("stt_prop_ramp_spiral_m") || objecthash == joaat("stt_prop_ramp_spiral_l_m") || objecthash == joaat("stt_prop_ramp_spiral_l") || objecthash == joaat("stt_prop_ramp_spiral_l_l") || objecthash == joaat("stt_prop_ramp_spiral_xxl") || objecthash == joaat("stt_prop_ramp_spiral_l_xxl") || objecthash == joaat("stt_prop_ramp_adj_hloop") || objecthash == joaat("stt_prop_wallride_01") || objecthash == joaat("stt_prop_wallride_01b") || objecthash == joaat("stt_prop_wallride_04") || objecthash == joaat("stt_prop_wallride_45r") || objecthash == joaat("stt_prop_wallride_45ra") || objecthash == joaat("stt_prop_wallride_45l") || objecthash == joaat("stt_prop_wallride_45la") || objecthash == joaat("stt_prop_wallride_90r") || objecthash == joaat("stt_prop_wallride_90rb") || objecthash == joaat("stt_prop_wallride_90l") || objecthash == joaat("stt_prop_wallride_90lb") || objecthash == joaat("stt_prop_wallride_02") || objecthash == joaat("stt_prop_wallride_02b") || objecthash == joaat("stt_prop_wallride_05b") || objecthash == -630267126 || objecthash == -1344401943 || objecthash == -1137432939 || objecthash == -1751978709 || objecthash == -1524398004 || objecthash == 2032545874 || objecthash == 552023728 || objecthash == 312351262 || objecthash == 1165295563 || objecthash == 1939128168 || objecthash == -2058034456 || objecthash == -1775238062 || objecthash == -411908812 || objecthash == 959604918 || objecthash == -875950621 || objecthash == 511619919 || objecthash == 690735273 || objecthash == 2019514094 || objecthash == 1490006141 || objecthash == -29366490 || objecthash == -1460937400 || objecthash == 2131364229 || objecthash == 949515150 || objecthash == -109574202 || objecthash == -1342281820 || objecthash == 1112939169 || objecthash == 1575467428 || objecthash == 472547144 || objecthash == 362681026 || objecthash == -1398962413)
            {
                return new string[] { "MC_STNT_USA", "MC_STNT_RED", "MC_STNT_BLU", "MC_STNT_PRP", "MC_STNT_BLK", "MC_STNT_WHT", "MC_STNT_GRY", "MC_STNT_YLW", "MC_STNT_ORG", "MC_STNT_GRN", "MC_STNT_PNK", "MC_STNT_RAC", "MC_STNT_BNY", "MC_STNT_ONB", "MC_STNT_GNY", "MC_STNT_PNG" };
            }
            else if (objecthash == joaat("stt_prop_hoop_tyre_01a") || objecthash == -291659998 || objecthash == 1673929480 || objecthash == 1708511539 || objecthash == -117245244 || objecthash == -60015937 || objecthash == -1576755324 || objecthash == -544828600 || objecthash == -558053832 || objecthash == -796663061 || objecthash == -816612831 || objecthash == -237619225 || objecthash == 1666830192 || objecthash == -838998190)
            {
                return new string[] { "MC_STNT_BLK", "MC_STNT_RED", "MC_STNT_BLU", "MC_STNT_PNK", "MC_STNT_GRY", "MC_STNT_WHT", "MC_STNT_GRY", "MC_STNT_YLW", "MC_STNT_ORG", "MC_STNT_GRN" };
            }
            else if (objecthash == joaat("stt_prop_corner_sign_01") || objecthash == joaat("stt_prop_corner_sign_02") || objecthash == joaat("stt_prop_corner_sign_03") || objecthash == joaat("stt_prop_corner_sign_04") || objecthash == joaat("stt_prop_corner_sign_05") || objecthash == joaat("stt_prop_corner_sign_06") || objecthash == joaat("stt_prop_corner_sign_07") || objecthash == joaat("stt_prop_corner_sign_08") || objecthash == joaat("stt_prop_corner_sign_09") || objecthash == joaat("stt_prop_corner_sign_10") || objecthash == joaat("stt_prop_corner_sign_11") || objecthash == joaat("stt_prop_corner_sign_12") || objecthash == joaat("stt_prop_corner_sign_13") || objecthash == joaat("stt_prop_corner_sign_14"))
            {
                return new string[] { "MC_STNT_BNW", "MC_STNT_YNR", "MC_STNT_RNW", "MC_STNT_YNB" };
            }
            else if (objecthash == -731135591)
            {
                return new string[] { "MC_STNT_USA", "MC_STNT_RNW", "MC_STNT_BLU", "MC_STNT_PRP", "MC_STNT_BLK", "MC_STNT_WHT", "MC_STNT_GRY", "MC_STNT_YLW", "MC_STNT_ORG", "MC_STNT_GRN", "MC_STNT_GNP", "MC_STNT_RAC", "MC_STNT_BNY", "MC_STNT_ONB", "MC_STNT_GNY", "MC_STNT_PNG" };
            }
            else
            {
                return new string[] { };
            }
        }

        private int joaat(string txt)
        {
            return Game.GenerateHash(txt);
        }
    }
}
