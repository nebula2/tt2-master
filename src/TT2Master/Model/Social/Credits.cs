using Prism.Mvvm;

namespace TT2Master.Model.Social
{
    /// <summary>
    /// Credits to contributors
    /// </summary>
    public class Credits : BindableBase
    {
        private string _name;
        /// <summary>
        /// Name of contributor
        /// </summary>
        public string Name { get => _name; set => SetProperty(ref _name, value); }

        private string _text;
        /// <summary>
        /// Text to honor contributor
        /// </summary>
        public string Text { get => _text; set => SetProperty(ref _text, value); }
    }
}