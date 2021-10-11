namespace AgilePackage.Web.App.Dtos
{
    public class VoteCardDto
    {
        public string Name { get; set; }
        public int? Vote { get; set; }

        public VoteCardDto()
        {

        }

        public VoteCardDto(string name, int? vote)
        {
            Name = name;
            Vote = vote;
        }
    }
}
