using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TT2Master.Shared.Helper;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TT2Master
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MSBarControl : ContentView
	{
        public string CurrentStageText { get; set; } = "1";

        public string MaxStageText { get; set; } = "1";

        public double CurrentPercentageText { get; set; } = 1;

        public static readonly BindableProperty CurrentStageTextProperty = BindableProperty.Create(
                                                         propertyName: nameof(CurrentStageText),
                                                         returnType: typeof(string),
                                                         declaringType: typeof(MSBarControl),
                                                         defaultValue: "1",
                                                         defaultBindingMode: BindingMode.TwoWay,
                                                         propertyChanged: CurrStageTextPropertyChanged);

        public static readonly BindableProperty MaxStageTextProperty = BindableProperty.Create(
                                                         propertyName: nameof(MaxStageText),
                                                         returnType: typeof(string),
                                                         declaringType: typeof(MSBarControl),
                                                         defaultValue: "1",
                                                         defaultBindingMode: BindingMode.TwoWay,
                                                         propertyChanged: MaxStageTextPropertyChanged);

        public static readonly BindableProperty CurrentPercentageTextProperty = BindableProperty.Create(
                                                         propertyName: nameof(CurrentPercentageText),
                                                         returnType: typeof(string),
                                                         declaringType: typeof(MSBarControl),
                                                         defaultBindingMode: BindingMode.TwoWay,
                                                         propertyChanged: CurrentPercentageTextPropertyChanged);

        public MSBarControl ()
		{
			InitializeComponent ();
		}

        private static void CurrStageTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (MSBarControl)bindable;
            if(control == null || newValue == null)
            {
                return;
            }

            control.currStage.Text = newValue.ToString();
        }

        private static void MaxStageTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (MSBarControl)bindable;

            if (control == null || newValue == null)
            {
                return;
            }

            control.maxStage.Text = newValue.ToString();
        }

        private static void CurrentPercentageTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (MSBarControl)bindable;

            if (control == null || newValue == null)
            {
                return;
            }

            control.progrBar.Progress = JfTypeConverter.ForceDoubleStandard(newValue.ToString(), 1);
        }
    }
}