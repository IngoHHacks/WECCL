namespace WECCL.API;

public static class Buttons
{
    public class Button
    {
        private string _originalText;
        public string Text;
        private Func<string> Action;
        internal bool Active = true;
        private bool ExtraConfirmation;
        private bool _confirmed;
        private int _keytim = 0;
        
        public Button(string text, Func<string> action, bool extraConfirmation = false)
        {
            _originalText = text;
            Text = text;
            Action = action;
            ExtraConfirmation = extraConfirmation;
        }
        
        internal void Invoke()
        {
            if (_keytim > 0)
            {
                return;
            }
            _keytim = 10;
            if (ExtraConfirmation && !_confirmed)
            {
                Text = "Are you sure?";
                _confirmed = true;
                return;
            }
            Text = Action.Invoke();
            Active = false;
            _confirmed = false;
        }
        
        internal void Reset()
        {
            Text = _originalText;
            Active = true;
            _confirmed = false;
            _keytim = 0;
        }
        
        internal void Update()
        {
            if (_keytim > 0)
            {
                _keytim--;
            }
        }
    }
    
    internal static Dictionary<string, List<Button>> CustomButtons = new();
    
    public static void RegisterCustomButton(this BaseUnityPlugin plugin, string text, Func<string> action, bool extraConfirmation = false)
    {
        var modName = plugin.Info.Metadata.Name;
        if (!CustomButtons.TryGetValue(modName, out var buttons))
        {
            buttons = new List<Button>();
            CustomButtons.Add(modName, buttons);
        }
        buttons.Add(new Button(text, action, extraConfirmation));
    }
}