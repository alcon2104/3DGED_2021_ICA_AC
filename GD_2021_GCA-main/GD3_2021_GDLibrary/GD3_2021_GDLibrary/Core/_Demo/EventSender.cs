namespace GDLibrary.Core.Demo
{
    public class EventSender
    {
        //store a pointer to a subscribing function outside of this class
        public delegate void PlayerEventHandler(string s, int x, float y);

        //event has flag (on/off), list of attached functions to notify
        public event PlayerEventHandler PlayerChanged;

        public EventSender()
        {
        }

        public void OnPlayerChanged()
        {
            PlayerChanged?.Invoke("hello", 21, 3.14f);
        }
    }
}