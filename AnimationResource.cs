namespace com.companyname.navigationgraph9net9
{
    internal class AnimationResource
    {
        public static int EnterAnimation { get; set; }
        public static int ExitAnimation { get; set; }
        public static int PopEnterAnimation { get; set; }
        public static int PopExitAnimation { get; set; }

        public static void Default()
        {
            EnterAnimation = Resource.Animation.nav_default_enter_anim;
            ExitAnimation = Resource.Animation.nav_default_exit_anim;
            PopEnterAnimation = Resource.Animation.nav_default_pop_enter_anim;
            PopExitAnimation = Resource.Animation.nav_default_pop_exit_anim;
        }
        public static void Slider()
        {
            EnterAnimation = Resource.Animation.slide_in_right;
            ExitAnimation = Resource.Animation.slide_out_left;
            PopEnterAnimation = Resource.Animation.slide_in_left;
            PopExitAnimation = Resource.Animation.slide_out_right;
        }

        public static void SliderFader()
        {
            EnterAnimation = Resource.Animation.slide_in_right;
            ExitAnimation = Resource.Animation.slide_out_left;
            //PopEnterAnimation = Resource.Animation.m3_motion_fade_enter;
            //PopExitAnimation = Resource.Animation.m3_motion_fade_exit;
            //PopEnterAnimation = Resource.Animation.nav_default_pop_enter_anim;      // Reasonable
            //PopExitAnimation = Resource.Animation.nav_default_pop_exit_anim;
            PopEnterAnimation = Resource.Animation.fade_in;                             // Better very little flashing.
            PopExitAnimation = Resource.Animation.fade_out;
        }

        //public static void Fader()
        //{
        //    EnterAnimation = Resource.Animation.fragment_fade_enter;
        //    ExitAnimation = Resource.Animation.fragment_fade_exit;
        //    PopEnterAnimation = Resource.Animation.fragment_fade_enter;
        //    PopExitAnimation = Resource.Animation.fragment_fade_exit;
        //}

        public static void Fader2()
        {
            EnterAnimation = Resource.Animation.abc_grow_fade_in_from_bottom;
            ExitAnimation = Resource.Animation.abc_shrink_fade_out_from_bottom;
            PopEnterAnimation = Resource.Animation.nav_default_pop_enter_anim;
            PopExitAnimation = Resource.Animation.nav_default_pop_exit_anim;
        }

        public static void Fader3()
        {
            EnterAnimation = Resource.Animation.m3_motion_fade_enter;
            ExitAnimation = Resource.Animation.m3_motion_fade_exit;
            PopEnterAnimation = Resource.Animation.m3_motion_fade_enter;
            PopExitAnimation = Resource.Animation.m3_motion_fade_exit;
        }

        public static void FastFader()
        {
            EnterAnimation = Resource.Animation.fade_in;
            ExitAnimation = Resource.Animation.fade_out;
            PopEnterAnimation = Resource.Animation.fade_in;
            PopExitAnimation = Resource.Animation.fade_out;
        }

        public static void Fader4()
        {
            EnterAnimation = Resource.Animation.abc_grow_fade_in_from_bottom;
            ExitAnimation = Resource.Animation.abc_shrink_fade_out_from_bottom;
            PopEnterAnimation = Resource.Animation.abc_grow_fade_in_from_bottom;
            PopExitAnimation = Resource.Animation.abc_shrink_fade_out_from_bottom;
        }
    }
}