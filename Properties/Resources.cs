using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace Properties
{
    internal class Resources
    {
        private static ResourceManager resourceMan;

        private static CultureInfo resourceCulture;

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager
        {
            get
            {
                if (Resources.resourceMan == null)
                {
                    Resources.resourceMan = new ResourceManager("IPTV_Checker_2._5.Properties.Resources", typeof(Resources).Assembly);
                }
                return Resources.resourceMan;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get => Resources.resourceCulture;
            set => Resources.resourceCulture = value;
        }

        internal Resources()
        {
        }
    }
}
