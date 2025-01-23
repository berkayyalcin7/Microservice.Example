namespace Coordinator.Models
{
    // Bir entity içerisinde Name ile servislerin isimlerini alacağız. Record
    public record Node(string Name)
    {
        public Guid Id { get; set; }

        public ICollection<NodeState> NodeStates { get; set; }
    }
}
