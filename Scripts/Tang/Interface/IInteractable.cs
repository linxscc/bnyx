namespace Tang
{
    public interface IInteractable
    {
        int State { set; get; }
        bool CanInteract();
        void Interact();
    }
}