namespace PoeHUD.Poe.EntityComponents
{
    public class Stack : Component
    {
        public int Size
        {
            get
            {
                if (this.address == 0)
                {
                    return 0;
                }
                int res = this.m.ReadInt(this.address + 12);
                return res;
            }
        }

        public Stack()
        {
        }
    }
}