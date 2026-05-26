using UnityEngine;

namespace Project.Episode2
{
    /// <summary>
    /// Contains static data and text content for Episode 2 mini-games.
    /// All content is in Mongolian for the target audience (ages 11-12).
    /// Episode theme: "Сэтгэл хөдлөлийн ертөнц" (World of Emotions)
    /// Focus: Understanding peer bullying and emotional responses
    /// </summary>
    public static class Episode2Data
    {
        #region Game 1: Puzzle Assembly Messages

        /// <summary>
        /// Messages revealed after completing each puzzle stage.
        /// These help children understand Niko's emotional journey.
        /// </summary>
        public static readonly string[] PuzzleMessages = new string[]
        {
            // Message 1
            "Сэтгэл хөдлөлөө таньснаар бусдад тусалж чадна.",
            // Translation: By recognizing your emotions, you can help others.

            // Message 2
            "Уурласан ч бурууг бусдад бүү тох - хэн нэгнийг цохиж, доромжлох нь буруу.",
            // Translation: Even when angry, don't blame others - hitting and insulting someone is wrong.

            // Message 3
            "Тайвшрахын тулд бусдыг гомдоох ёсгүй.",
            // Translation: You shouldn't hurt others to calm down.

            // Message 4
            "Жинхэнэ хүч гэдэг нь зөвхөн биеийн чадал биш сэтгэл хөдлөлөө удирдаж чаддаг байх юм."
            // Translation: True strength is not just physical ability, but being able to manage your emotions.
        };

        /// <summary>
        /// Instruction text shown at the start of the puzzle game.
        /// </summary>
        public const string PuzzleInstruction = "Найзуудаа Нико-д сэтгэл хөдлөлөө таньж ойлгоход нь тусалцгаая!";
        // Translation: Friends, let's help Niko recognize and understand his emotions!

        /// <summary>
        /// Voice instruction for assembling pieces.
        /// </summary>
        public const string PuzzleVoiceInstruction = "Дүрсийг эвлүүлээрэй";
        // Translation: Assemble the pieces

        #endregion

        #region Game 2: Color Wheel Memory - Emotion Messages

        /// <summary>
        /// Introduction text for the memory game.
        /// Spoken by Jiin character.
        /// </summary>
        public const string MemoryGameIntro = "Найзуудаа хамтдаа үе тэнгийн дээрэлхэлтийг харсан даруйдаа бидний сэтгэл хөдлөлүүд тэр үйлдлийг хэрхэн мэдэрч хариу үйлдэл үзүүлж болохыг харцгаая";
        // Translation: Friends, let's see together how our emotions perceive and respond to peer bullying when we witness it.

        /// <summary>
        /// Instruction for the memory sequence.
        /// </summary>
        public const string MemoryGameInstruction = "Өнгөний дарааллыг сайн цээжлээрэй. Тэгээд дарааллын дагуу дараарай";
        // Translation: Memorize the color sequence well. Then press them in order.

        /// <summary>
        /// Messages for each emotion (spoken by Lulu character).
        /// </summary>
        public static class EmotionMessages
        {
            // Red - Anger (Уурлах)
            public const string Anger = "Бусдыг дээрэлхэж буйг хараад уур хүрдэг. Энэ уур бол зүгээр нэг уцаар биш \"шударга бус\" байдалд дургүйцэж буйн илрэл. Уурласан үедээ ч зөв зүйл хийж, тусламж дуудаж чадна.";
            // Translation: Seeing someone being bullied makes you angry. This anger is not just irritation, but an expression of disliking "unfairness". Even when angry, you can do the right thing and call for help.

            // Blue - Sadness (Гуниглах)
            public const string Sadness = "Хэн нэгнийг дээрэлхүүлж байхыг харах үнэхээр гунигтай. Гэхдээ энэ гуниг бидэнд: \"Би ийм зүйлийг зүгээр өнгөрөөмөөргүй байна\" гэж сануулж буй дохио.";
            // Translation: Watching someone being bullied is truly sad. But this sadness is a signal telling us: "I don't want to just let this pass."

            // Purple - Fear (Айх)
            public const string Fear = "Хэн нэгнийг хамгаалахыг хүссэн ч \"Би өөрөө дээрэлхүүлж магадгүй\" хэмээн айх мэдрэмж төрдөг. Гэхдээ зөв хүнээс /Багш, өөрийн итгэдэг том хүн/-с тусламж хүсэх нь айдсыг давж буй зориг юм.";
            // Translation: Even if you want to protect someone, a feeling of fear arises thinking "I might get bullied too." But asking for help from the right person (teacher, trusted adult) is courage that overcomes fear.

            // Orange - Surprise (Гайхах)
            public const string Surprise = "Хэн нэгнийг дээрэлхүүлж буйг хараад \"Яагаад ийм зүйл болж байна вэ\" гэж гайхаж магадгүй. Гайхах мэдрэмж нь бидэнд ойлгох, асуух, юу зөв болохыг эргэцүүлэх боломжийг олгодог.";
            // Translation: Seeing someone being bullied, you might wonder "Why is this happening?" The feeling of surprise gives us the opportunity to understand, ask, and reflect on what is right.

            // Yellow - Joy (Баярлах)
            public const string Joy = "Хэн нэгэнд тусалж, шударга зүйл хийсний дараа төрөх баяр хөөр. Өөрийн хийсэн зөв үйл бусдад бага ч гэсэн гэрлийг түгээж чадна.";
            // Translation: The joy that comes after helping someone and doing the right thing. Your right action can spread even a little light to others.

            // Green - Disgust (Жигших)
            public const string Disgust = "Бусдыг дээрэлхэж, муухай үг хэлж буйг хөндлөнгөөс харахад дотроос чинь \"энэ үйлдэл хэзээ ч зөв биш\" гэсэн мэдрэмж төрдөг. Энэ үед жигшсэнээ бусдаас нуух биш, харин мэдрэмжээ шударга зөв үйлийг дэмжихэд ашиглаарай.";
            // Translation: When you see someone being bullied or saying mean things, a feeling arises inside that "this action is never right." At this time, don't hide your disgust from others, but use your feelings to support fair and right actions.
        }

        #endregion

        #region Game 3: Story Quiz - Questions and Answers

        /// <summary>
        /// Quiz question data for the Bear and Deer story.
        /// </summary>
        public static class QuizData
        {
            // Question 1
            public const string Q1_Scenario = "Бүлэг баавгай бугыг тойрон зарим нь инээж нэг нь бугыг цохиж буй нь дүрслэгдэнэ.";
            // Translation: A group of bears surrounds a deer, some laughing while one hits the deer.

            public const string Q1_Question = "Энэ зураг ямар төрлийн дээрэлхэлтийг илэрхийлж байна вэ?";
            // Translation: What type of bullying does this picture represent?

            public static readonly string[] Q1_Answers = new string[]
            {
                "Бие махбодын дээрэлхэлт",      // Physical bullying
                "Үгийн дээрэлхэлт",              // Verbal bullying
                "Нийгмийн (харилцааны) дээрэлхэлт", // Social (relational) bullying
                "Цахим дээрэлхэлт"               // Cyberbullying
            };
            public const int Q1_CorrectAnswer = 0; // Physical bullying

            // Question 2
            public const string Q2_Scenario = "Ангидаа буга суух ба эргэн тойрон дахь бусад нь түүнрүү муухай харж, илт дургүйлхэн байна.";
            // Translation: A deer sits in class while others around look at it meanly, clearly showing dislike.

            public const string Q2_Question = "Хэрвээ чи ангидаа ийм явдал харвал бусдадаа юу гэж хэлж уриалах вэ?";
            // Translation: If you see such a thing in your class, what would you say to call on others?

            public static readonly string[] Q2_Answers = new string[]
            {
                "\"Зүгээр дээ, тоох хэрэггүй\"",           // "It's fine, no need to pay attention"
                "\"Бүгд нэг ангийнхан байж нэгнээ ингэж дээрэлхэхээ больё оо\"", // "We're all classmates, let's stop bullying each other"
                "\"Түүнийг битгий сандалд суулгаарай\"",   // "Don't let them sit in the chair"
                "\"Надаас л холуур байвал бусад нь хамаа алга өө\"" // "As long as they're away from me, I don't care about others"
            };
            public const int Q2_CorrectAnswer = 1; // "We're all classmates..."

            // Question 3
            public const string Q3_Scenario = "Ангийн групп чатанд бугыг шоолж дооглон элдэвлэн инээлдсэн агуулгатай чат харагдана.";
            // Translation: In the class group chat, messages mocking and ridiculing the deer can be seen.

            public const string Q3_Question = "Хэрвээ хэн нэгэн цахим орчинд чамайг эсвэл найзыг чинь муулж байвал чи юу хийх ёстой вэ?";
            // Translation: If someone is insulting you or your friend online, what should you do?

            public static readonly string[] Q3_Answers = new string[]
            {
                "Тэр пост болон чатыг бусадтай хуваалцана",    // Share that post and chat with others
                "Хариуд нь бас дэмжсэн эможи явуулна",          // Send supportive emojis in response
                "Скриншот хийн том хүнд (эцэг эх, багшид) үзүүлэн арга хэмжээ авах", // Take a screenshot and show it to an adult (parent, teacher) to take action
                "Би л биш бол чимээгүй л байна"                 // If it's not me, I'll just stay quiet
            };
            public const int Q3_CorrectAnswer = 2; // Take screenshot and show adult

            // Feedback messages
            public const string CorrectFeedback = "Зөв байна.";
            // Translation: Correct.

            public const string WrongFeedback = "Зургийг сайн ажиглаарай.";
            // Translation: Look at the picture carefully.
        }

        #endregion

        #region Bullying Types

        /// <summary>
        /// Types of bullying covered in Episode 2.
        /// </summary>
        public enum BullyingType
        {
            Physical,   // Бие махбодын дээрэлхэлт
            Verbal,     // Үгийн дээрэлхэлт
            Social,     // Нийгмийн (харилцааны) дээрэлхэлт
            Cyber       // Цахим дээрэлхэлт
        }

        #endregion
    }
}
