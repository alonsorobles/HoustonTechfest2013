using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Document;

namespace HelpDeskThingamabob
{
    class Program
    {
        static void Main(string[] args)
        {
            using (
                var documentStore =
                    new DocumentStore {Url = "http://localhost:8080", DefaultDatabase = "HoustonTechFest2013"}
                        .Initialize())
            {
                using (var session = documentStore.OpenSession())
                {
                    // Inserting data
                    var ticket = new Ticket
                        {
                            SubmissionTime = DateTime.UtcNow,
                            Description = "Speaker doesn't listen",
                            Severity = Severity.Worse,
                            Submitter = new Person
                                {
                                    Name = "Tom Wilferd",
                                    Phone = "1 (800) HOT-STUF"
                                }
                        };
                    session.Store(ticket);

                    // Load and Update
                    var oldTicket = session.Load<Ticket>("tickets/1");
                    oldTicket.Submitter = new Person {Name = "Alonso Robles", Phone = "1(800)HOTTER1"};
                    
                    // Query and Delete
                    var tickets = session.Query<Ticket>().Where(t => t.Id.StartsWith("tickets")).Take(100);
                    foreach (var thingy in tickets)
                    {
                        if (thingy.Id == "tickets/20")
                        {
                            session.Delete(thingy);
                        }
                    }

                    var badTickets = session.Query<Ticket>().Where(t => t.Severity == Severity.Bad);

                    foreach (var badTicket in badTickets)
                    {
                        badTicket.Notes = "I was very very bad!";
                    }

                    session.SaveChanges();

                }
            }
        }
    }

    public class Ticket
    {
        public string Id { get; set; }
        public DateTime SubmissionTime { get; set; }
        public string Description { get; set; }
        public Severity Severity { get; set; }
        public Person Submitter { get; set; }
        public string Notes { get; set; }
    }

    public class Person
    {
        public string Name { get; set; }
        public string Phone { get; set; }        
    }
    
    public enum Severity
    {
        Bad,
        Worse,
        OhMy
    }
}
