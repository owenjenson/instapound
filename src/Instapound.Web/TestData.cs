using Instapound.Data;
using Instapound.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Instapound.Web;

// Data vygenerovalo Gemini

internal static class TestData
{
    const string DefaultPassword = "Test1234";

    public static async void SeedData(AppDbContext context, UserManager<User> userManager)
    {
        var users = GetUsers();
        var userNames = users.Select((u) => u.UserName).ToList();

        if (context.Users.Any((u) => userNames.Contains(u.UserName)))
            return;

        foreach (var user in users)
            await userManager.CreateAsync(user, DefaultPassword);

        users = context.Users.ToList();

        var posts = GetPosts(users);
        context.Posts.AddRange(posts);

        var comments = GetComments().Select((text) =>
        {
            var random = new Random();
            var post = GetRandomItem(posts);
            var author = GetRandomItem(users, post.Author);

            return new Comment
            {
                Text = text,
                CreatedAt = new DateTime(DateTime.Now.Ticks - random.NextInt64(DateTime.Now.Ticks - post.ReleasedAt.Ticks)),
                Author = author,
                Post = post,
            };
        }).ToList();
        context.Comments.AddRange(comments);

        foreach (var user in users.Take(5))
            user.Following.AddRange(GetRandomItems(users, user, 1));
        foreach (var user in users.Skip(5))
            user.FollowedBy.AddRange(GetRandomItems(users, user));
        foreach (var user in users.Skip(1))
            if (!user.FollowedBy.Contains(users[0]))
                user.FollowedBy.Add(users[0]);

        foreach (var user in users)
        {
            var random = new Random();

            user.SentMessages.AddRange(GetRandomItems(users, user)
                .SelectMany((userTo) => GetRandomItems(GetMessages(), null, 0, 5)
                    .Select((message) => new Message
                    { 
                        Text = message,
                        CreatedAt = new DateTime(DateTime.Now.Ticks - random.NextInt64(TimeSpan.FromDays(30).Ticks)),
                        FromUser = user,
                        ToUser = userTo,
                    })));
        }

        foreach (var post in posts)
        {
            post.TaggedUsers.AddRange(GetRandomItems(users, post.Author, 0, 2));
            post.LikedByUsers.AddRange(GetRandomItems(users, post.Author, 1));
        }

        foreach (var comment in comments)
        {
            comment.LikedByUsers.AddRange(GetRandomItems(users, comment.Author, 0, 3));
        }

        context.SaveChanges();
    }

    static List<User> GetUsers() =>
        [
            new User
            {
                UserName = "jano",
                PasswordHash = "",
                Avatar = "jano.jpg",
                Bio = "Zachycuji okamžiky, které stojí za zapamatování. ✨",
                FirstName = "Jan",
                LastName = "Nový",
                DateOfBirth = DateTime.Today.AddYears(-30),
            },
            new User
            {
                UserName = "free_jana",
                PasswordHash = "",
                Avatar = "free_jana.jpg",
                Bio = "😶‍🌫️ Ztracena v jiných světech. Knihy jsou moje útočiště 📚",
                FirstName = "Jana",
                LastName = "Svobodová",
                DateOfBirth = DateTime.Today.AddYears(-25)
            },
            new User
            {
                UserName = "dvorak",
                PasswordHash = "",
                Avatar = "dvorak.jpg",
                Bio = "Neustále se učím, rostu a inspiruji ostatní 🦠 🐙",
                FirstName = "Michal",
                LastName = "Dvořák",
                DateOfBirth = DateTime.Today.AddYears(-40)
            },
            new User
            {
                UserName = "em-cerna",
                PasswordHash = "",
                Avatar = "em-cerna.jpg",
                Bio = "Moje myšlenky, zážitky a každodenní život. Přidej se ke mně!",
                FirstName = "Emily",
                LastName="Černá",
                DateOfBirth = DateTime.Today.AddYears(-35)
            },
            new User
            {
                UserName = "david6378",
                PasswordHash = "",
                Avatar = "david6378.jpg",
                Bio = "🌏 Bágl na zádech a svět před sebou 🌍 Sdílím své zážitky z cest 🗺️",
                FirstName = "David",
                LastName = "Novotný",
                DateOfBirth = DateTime.Today.AddYears(-28)
            },
            new User
            {
                UserName = "sarah",
                PasswordHash = "",
                Avatar = "sarah.jpg",
                Bio = "Snídám v Praze, obědvám v Římě a večeřím v Tokiu. ✈️",
                FirstName = "Sarah",
                LastName = "Marková",
                DateOfBirth = DateTime.Today.AddYears(-22)
            },
            new User
            {
                UserName = "neuman",
                PasswordHash = "",
                Avatar = "neuman.jpg",
                FirstName = "Kryštof",
                LastName = "Neuman",
                DateOfBirth = DateTime.Today.AddYears(-38)
            },
            new User
            {
                UserName = "0liva",
                PasswordHash = "",
                Avatar = "0liva.jpg",
                FirstName = "Olivia",
                LastName = "Procházková",
                DateOfBirth = DateTime.Today.AddYears(-27)
            },
            new User
            {
                UserName = "kuba",
                PasswordHash = "",
                Avatar = "kuba.jpg",
                Bio = "Fuelled by caffeine and good vibes ☕️✨",
                FirstName = "Jakub",
                LastName = "Svoboda",
                DateOfBirth = DateTime.Today.AddYears(-33)
            },
            new User
            {
                UserName = "sofie01",
                PasswordHash = "",
                Avatar = "sofie01.jpg",
                Bio = "Always seeking new adventures and experiences 🌍✈️",
                FirstName = "Sofie",
                LastName = "Adamcová",
                DateOfBirth = DateTime.Today.AddYears(-24)
            },
        ];

    static List<Post> GetPosts(List<User> users) =>
        [
            new Post
            {
                ReleasedAt = DateTime.Now.AddDays(-5),
                Text = "Nejnovější technologické trendy:",
                Image = "tech_trends.jpg",
                Author = users[0],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddDays(-6),
                Text = "Leonardo da Vinci byl nejen malíř, ale také vynálezce.",
                Author = users[0],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddHours(-6),
                Text = "Ve vesmíru není žádný zvuk, protože tam není prostředí, kterým by se šířil.",
                Author = users[1],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddHours(-16),
                Text = "Podívej se na tuto úžasnou cestovní destinaci!",
                Image = "travel_destination.jpg",
                Author = users[1],
            },
            new Post
            {
                ReleasedAt = new DateTime(2024, 1, 1),
                Text = "Já, když se snažím být produktivní, ale místo toho scrolluji na Instagramu.",
                Image = "scrolling.jpg",
                Author = users[0],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddHours(-1),
                Text = "Káva a knížka - dokonalá kombinace pro odpoledne",
                Author = users[2],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddDays(-7),
                Text = "Mravenci dokážou zvednout až 50násobek své vlastní váhy.",
                Image = "ant.jpg",
                Author = users[7],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddDays(-1),
                Text = "Nejstarší strom na světě je starý přes 5000 let.",
                Author = users[7],
            },
            new Post
            {
                ReleasedAt = new DateTime(2024, 7, 4),
                Text = "Já, když se snažím být dospělý, ale pořád si hraju s filtrama na Instagramu.",
                Image = "filters.jpg",
                Author = users[8],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddDays(-8),
                Text = "Nový outfit, nové já! Jak se vám líbí moje dnešní kombinace?",
                Image = "outfit.jpg",
                Author = users[0],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddDays(-16),
                Text = "Každý den ztratíme asi 40-80 vlasů.",
                Author = users[0],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddHours(-37),
                Text = "Slavím Den matek se speciálním vzkazem",
                Image = "mothers_day.jpg",
                Author = users[8],
            },
            new Post
            {
                ReleasedAt = new DateTime(2024, 6, 21),
                Text = "Každý den je nová příležitost začít znovu.",
                Author = users[1],
            },
            new Post
            {
                ReleasedAt = new DateTime(2024, 7, 4),
                Text = "Věděli jste, že kočky mají přes 32 svalů v každém uchu?",
                Author = users[0],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddDays(-1),
                Text = "Největší strom na světě je sekvoje obrovská.",
                Author = users[6],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddHours(-78),
                Text = "Je tu léto! Je čas vyrazit na pláž",
                Image = "beach_vacation.jpg",
                Author = users[5],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddHours(-12),
                Text = "Chameleoni mění barvu nejen kvůli maskování, ale také kvůli náladě.",
                Image = "chameleon.jpg",
                Author = users[2],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddDays(-32),
                Text = "Každý člověk má jedinečný otisk jazyka.",
                Author = users[2],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddHours(-32),
                Text = "Koala spí až 22 hodin denně a živí se výhradně listy eukalyptu.",
                Author = users[2],
            },

            new Post
            {
                ReleasedAt = DateTime.Now.AddHours(-456),
                Text = "The human brain can generate more electrical activity than a city 🤯",
                Image = "brain.jpg",
                Author = users[8],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddDays(-12),
                Text = "The first-ever vending machine dispensed holy water",
                Author = users[8],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddHours(-3),
                Text = "The longest recorded flight of a chicken was 13 seconds 🐔",
                Image = "chicken.jpg",
                Author = users[8],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddDays(-326),
                Text = "The average person laughs about 15 times a day 🤣",
                Author = users[8],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddHours(-125),
                Text = "The average person walks the equivalent of three times around the Earth in their lifetime",
                Image = "earth.jpg",
                Author = users[8],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddHours(-86),
                Text = "The world's oldest known piece of chewing gum is over 9,000 years old",
                Author = users[8],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddHours(-180),
                Text = "The first recorded use of the word \"robot\" was in a 1920 Czech play 🤖",
                Image = "robot.jpg",
                Author = users[8],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddHours(-1),
                Text = "The world's largest desert, Antarctica, covers over 5.5 million square miles ☃️⛄",
                Image = "antarctica.jpg",
                Author = users[7],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddHours(-64),
                Text = "The world's deepest lake, Lake Baikal, holds 20% of the world's fresh surface water 🌊",
                Author = users[7],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddHours(-213),
                Text = "The world's smallest country, Vatican City, has the lowest crime rate",
                Author = users[7],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddHours(-14),
                Text = "Na Marsu se nachází nejvyšší hora ve sluneční soustavě, Olympus Mons.",
                Image = "mars.jpg",
                Author = users[2],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddDays(-34),
                Text = "Nejvyšší budova světa je Burj Khalifa v Dubaji.",
                Author = users[2],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddDays(-300),
                Text = "Voda pokrývá více než 70 % zemského povrchu 🐳",
                Author = users[3],
            },
            new Post
            {
                ReleasedAt = DateTime.Now.AddDays(-155),
                Text = "Nejbližší hvězda k Zemi (kromě Slunce) je Proxima Centauri 🌞",
                Author = users[3],
            },
        ];

    static List<string> GetComments() =>
        [
            "Skvělý příspěvek!",
            "Souhlasím",
            "Nesouhlasím",
            "Neuvěřitelný talent!",
            "Tohle je zajímavé",
            "Můžeš to prosím rozvést?",
            "Mám dotaz",
            "Nejsem si tím jistý",
            "Jak dlouho ti to trvalo?",
            "Jak jsi to vytvořila?",
            "Taky bych tam chtěla být ✈️",
            "Chtěla bych se dozvědět víc",
            "Tohle je dobře napsaný",
            "Krásná fotka!",
            "Hodně jsem se naučil",
            "Tohle je cenný příspěvek",
            "Jsem z toho zklamaný",
            "Já tohle miluju! ❤️",
            "Krásné barvy!",
            "Jaký je tvůj oblíbený nástroj?",
            "Tohle je ztráta času",
            "Kde jsi to našel?",
            "Zajímavý pohled",
            "Potrava pro myšlenky",
            "Je potřeba více výzkumu",
            "Tohle je komplexní problém",
            "Inspiruješ mě! ✨",
            "Myslím, že to je dobrý nápad",
            "Nicméně mám určité obavy",
            "Proto bychom to měli zvážit",
            "To je naprosto úžasné! ",
            "Souhlasím na 100%",
            "Nejlepší příspěvek!",
            "Tohle je přesně to, co potřebuju vidět",
            "Co tě k tomu inspirovalo?",
            "Mohlo to být lepší",
            "Great post!",
            "I agree",
            "I disagree",
            "Incredible talent!",
            "This is interesting",
            "Can you please elaborate?",
            "I have a question",
            "I'm not sure about that",
            "How long did that take?",
            "How did you create this?",
            "I want to be there too ✈️",
            "I'd like to learn more",
            "This is well-written",
            "Beautiful photo!",
            "I learned a lot",
            "This is a valuable contribution",
            "I'm disappointed",
            "I love this! ❤️",
            "Beautiful colors!",
            "What's your favorite tool?",
            "This is a waste of time",
            "Where did you find this?",
            "Interesting perspective",
            "Food for thought",
            "More research is needed",
            "This is a complex problem",
            "You inspire me! ✨",
            "I think that's a good idea",
            "However, I have some concerns",
            "Therefore, we should consider this",
            "That's absolutely amazing!",
            "I agree 100%",
            "Best post ever!",
            "This is exactly what I needed to see",
            "What inspired you?",
            "❤️",
            "🐙",
            "🤔",
            "🤣",
            "😶",
            "🤯",
            "😊",
            "😂",
            "💩",
            "😍",
            "😴",
            "😭",
            "😱",
            "🙈",
            "🙉",
            "🙊",
        ];

    static List<string> GetMessages() =>
        [
            "Ahoj, co je nového?",
            "Jak se máš?",
            "Věříš tomu?",
            "Viděl jsi tu novinku?",
            "Přemýšlím o...",
            "Tak strašně se nudím!",
            "Potřebuju si s tebou o něčem promluvit",
            "Jsem fakt ve stresu",
            "Teď se cítím fakt šťastně",
            "Teď se cítím fakt smutně",
            "Nic moc, jen tak chilluju",
            "Mám se celkem dobře, a ty?",
            "Jo, to je šílený!",
            "Ne, neviděla jsem to. O čem to bylo?",
            "Co tě trápí?",
            "Taky!",
            "Jasně, co je?",
            "Chceš si o tom promluvit?",
            "To je skvělé slyšet!",
            "To mě mrzí",
            "❤️",
            "🐙",
            "🤔",
            "🤣",
            "😶",
            "🤯",
            "😊",
            "😂",
            "💩",
            "😍",
            "😴",
            "😭",
            "😱",
            "🙈",
            "🙉",
            "🙊",
            "╰(*°▽°*)╯",
            "O_O",
            "(⊙_◎)",
            "⚆_⚆",
        ];

    static T GetRandomItem<T>(IEnumerable<T> items, T? ignoredItem = null) where T : class
    {
        return items
            .Where(item => item != ignoredItem)
            .OrderBy(item => Guid.NewGuid())
            .First();
    }

    static IEnumerable<T> GetRandomItems<T>(List<T> items, T? ignoredItem = null, int minCount = 0, int? maxCount = null) where T : class
    {
        var random = new Random();
        var count = random.Next(minCount, maxCount + 1 ?? items.Count);

        return items
            .Where(item => item != ignoredItem)
            .OrderBy(item => Guid.NewGuid())
            .Take(count);
    }
}