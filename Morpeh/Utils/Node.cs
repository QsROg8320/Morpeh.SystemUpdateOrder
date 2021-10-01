namespace Morpeh.Utils
{
    public class Node<T> 
    {
        public T Value { get;  set; }
        public State State { get;  set; }
        public Node(T value, State state)
        {
            Value = value;
            State = state;
        }
    }
}
