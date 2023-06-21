using Sitecore.Data;

namespace Canvas.Project.Workshop {
public struct Templates {
    public struct HomePage {
        public static readonly ID ID = new ID("{57119F1F-34AC-41AD-8C1B-48D44DA900A5}");

        public struct Fields {
            public static readonly ID HeroImage = new ID("{19FA38CA-EB8B-47AA-B8D2-5F76E4550B19}");
            public static readonly ID HeroText = new ID("{873CE48B-B161-4F2F-8232-B32A1186845C}");
            public static readonly ID Body = new ID("{ADDA1D3D-A1F3-4DBE-8676-99592F2E6D31}");
        }
    }

    public struct ContactUsPage {
        public static readonly ID ID = new ID("{CB760FF4-2E01-47C7-97BA-CAFFE290F38E}");

        public struct Fields {
            public static readonly ID Email = new ID("{C864517C-74BF-412B-9BA3-06EE1CDC5854}");
            public static readonly ID PhoneNumber = new ID("{EC5A1F40-D9B0-464A-93F4-73D1C69DA7D2}");
            public static readonly ID Address = new ID("{95FA8B87-A991-46E2-AD01-06D9D32C0D47}");
        }
    }
}
}