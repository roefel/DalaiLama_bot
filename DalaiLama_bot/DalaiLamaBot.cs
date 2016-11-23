using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Audio;
using Discord.Modules;

namespace DalaiLama_bot
{
    class DalaiLamaBot
    {
        string[] compliments = new string[60];
        Random rnd = new Random();

        // variables for a poll
        List<string> answers = new List<string>();
        List<User> alreadyVoted = new List<User>();
        int answerPosition;
        string pollResults;
        string pollQuestion;
        string pollStart;
        bool hasAlreadyVoted;
        bool isPollCreated;
        bool isPollStarted;
        int[] answerVotes;
        User pollCreator;
        string voteStats;

        DiscordClient discord; //declaring an object of DiscordClient

        public DalaiLamaBot()
        {
            //array of compliments
            compliments[0] = "Your smile is contagious."; compliments[1] = "You look great today."; compliments[2] = "You're a smart cookie."; compliments[3] = "I bet you make babies smile."; compliments[4] = "I like your style."; compliments[5] = "You have the best laugh."; compliments[6] = "I appreciate you, you've never failed to make me smile."; compliments[7] = "You are the most perfect you there is."; compliments[8] = "You're an awesome friend."; compliments[9] = "You light up the server."; compliments[10] = "You deserve a hug right now."; compliments[11] = "You should be proud of yourself."; compliments[12] = "You're more helpful than you realize."; compliments[13] = "You have a great sense of humor."; compliments[14] = "Is that your picture next to \"charming\" in the dictionary?"; compliments[15] = "Your kindness is a blessing to all who encounter it."; compliments[16] = "On a scale from 1 to 10, you're an 11."; compliments[17] = "You are brave, and that makes you incredible."; compliments[18] = "You're even more beautiful on the inside than you are on the outside."; compliments[19] = "Your eyes are breathtaking."; compliments[20] = "You are having a positive impact on the world."; compliments[21] = "You're like sunshine on a rainy day."; compliments[22] = "You bring out the best in other people."; compliments[23] = "You're a great listener."; compliments[24] = "How is it that you always look great, even in sweatpants?"; compliments[25] = "Everything would be better if more people were like you!"; compliments[26] = "I bet you sweat glitter."; compliments[27] = "When you're not afraid to be yourself is when you're most incredible."; compliments[28] = "Colors seem brighter when you're around.";
            compliments[29] = "You're wonderful."; compliments[30] = "Jokes are funnier when you tell them."; compliments[31] = "You're better than a triple-scoop ice cream cone. With sprinkles."; compliments[32] = "Your hair looks stunning."; compliments[33] = "You're one of a kind!"; compliments[34] = "You're inspiring."; compliments[35] = "You should be thanked more often. So thank you!!"; compliments[36] = "Our community is better because you're in it."; compliments[37] = "Someone is getting through something hard right now because you've got their back. "; compliments[38] = "Everyone gets knocked down sometimes, but you always get back up and keep going."; compliments[39] = "You're a candle in the darkness"; compliments[40] = "You're a great example to others."; compliments[41] = "You are like a spring flower, beautiful and vivacious."; compliments[42] = "You're more fun than bubble wrap."; compliments[43] = "Your voice is magnificent."; compliments[44] = "The people you love are lucky to have you in their lives."; compliments[45] = "Any team would be lucky to have you on it."; compliments[46] = "If you were a booger, I'd pick you."; compliments[47] = "In high school I bet you were voted \"most likely to keep being awesome.\""; compliments[48] = "You're someone's reason to smile."; compliments[49] = "You're even better than a unicorn, because you're real."; compliments[50] = "The way you treasure your loved ones is incredible."; compliments[51] = "You're really something special."; compliments[52] = "You're a gift to those around you."; compliments[53] = " I find you to be a fountain of inspiration."; compliments[54] = "You really deserve a compliment! Because you are simply awesome."; compliments[55] = "I really enjoy spending time with you."; compliments[56] = "If I freeze, it's not a computer virus.  I was just stunned by your beauty."; compliments[57] = "If I freeze, it's not a computer virus.  I was just stunned by your beauty."; compliments[58] = "I'd like to know why you're so beautiful."; compliments[59] = "I just wanted you to know that you're a fantastic human being and let no one tell you otherwise!";

            discord = new DiscordClient(x =>    //discord log stuff
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = log;
            });

            discord.UsingAudio(x => //some audio stuff not in use yet
            {
                x.Mode = AudioMode.Outgoing;
            });

            discord.UsingCommands(x =>  //defining the prefix for a command and allowing mentions of the bot instead of a prefix
            {
                x.PrefixChar = '!';
                x.AllowMentionPrefix = true;
            });

            //var commands for using the command service of discord.net to create commands
            var commands = discord.GetService<CommandService>();

            //command for anonymous confessions
            commands.CreateCommand("confess")
                .Alias(new string[] { "confession" })
                .Parameter("confession", ParameterType.Unparsed)
                .Do(async (e) =>

            {
                await e.Message.Delete();
                await e.Server.GetChannel(248177041847877634).SendMessage(e.GetArg("confession"));
            });

            //command for giving a random compliment to a user
            commands.CreateCommand("compliment")
                .Parameter("x", ParameterType.Required)
                .Do(async (e) =>

             {
                 await e.Channel.SendMessage(compliment() + " " + e.GetArg("x"));
             });


            //command to get all roles their ID's
            commands.CreateCommand("getrolesid")
                .Do(async (e) =>
                {
                    string roles ="";
                    IEnumerable<Role> lol = e.Server.Roles;
                    foreach (Role role in lol)
                    {
                        roles += role.Name + ": " + role.Id.ToString() + "\n";

                    }
                    await e.User.SendMessage(roles);
                });

            //commandgroup to create a poll
            commands.CreateGroup("poll", cgb =>
            {   
                cgb.CreateCommand("create").    //create a poll
                    Parameter("x", ParameterType.Unparsed)
                    .Do(async (e) =>
                    {
                        if (!isPollCreated)
                        {
                            await e.Message.Delete();
                            pollCreator = e.Message.User;
                            Console.WriteLine("poll created");
                            isPollCreated = true;
                            isPollStarted = false;
                            answerPosition = 0;
                            pollQuestion = e.GetArg("x");
                            await e.Channel.SendMessage("A poll has been created with question: " + pollQuestion);
                        }
                        else
                        {
                            commandDenied(e.Channel);
                        }
                    });

                cgb.CreateCommand("answer").    //add answers to the question
                    Parameter("x", ParameterType.Unparsed).
                    Do(async (e) =>
                    {
                        if (isPollCreated && !isPollStarted && pollCreator == e.Message.User)
                        {
                            await e.Message.Delete();
                            string answer = e.GetArg("x");
                            Console.WriteLine("Answer has been added");
                            addAnswerToList(answer);
                            await e.Channel.SendMessage("The answer: *" + answers[answerPosition - 1] + "*\tHas been assigned to answer[" + answerPosition + "]");
                        }
                        else if (isPollCreated && !isPollStarted && pollCreator != e.Message.User)
                        {
                            await e.Channel.SendMessage("You're not the one that has created the poll");
                        }
                        else
                        {
                            commandDenied(e.Channel);
                        }
                    });
                cgb.CreateCommand("start"). //start the voting
                Do(async (e) =>
                {
                    if (isPollCreated && answers.Count >= 2 && !isPollStarted && pollCreator == e.Message.User)
                    {
                        await e.Message.Delete();
                        answerVotes = new int[answers.Count];
                        isPollStarted = true;
                        pollStart += "The poll: *" + pollQuestion + "* has started.\n";
                        for (int i = 0; i < answers.Count; i++)
                        {
                            pollStart += "answer [" + (i + 1) + "]:\t*" + answers[i] + "*\n";
                        }
                        await e.Channel.SendMessage(pollStart);
                    }
                    else if (isPollCreated && !isPollStarted && pollCreator != e.Message.User)
                    {
                        await e.Channel.SendMessage("You're not the one that has created the poll");
                    }
                    else if (isPollCreated)
                    {
                        await e.Channel.SendMessage("You need at least 2 answers");
                    }
                    else
                    {
                        commandDenied(e.Channel);
                    }
                });
                cgb.CreateCommand("vote").  //vote for an answer
                Parameter("x", ParameterType.Required).
                Do(async (e) =>
                {
                    int answerNumber;
                    int.TryParse(e.GetArg("x"), out answerNumber);

                    if (isPollStarted)
                    {
                        if (answerNumber <= answers.Count)
                        {
                            foreach(User user in alreadyVoted)
                            {
                                if (alreadyVoted.Contains(e.User))
                                {
                                    hasAlreadyVoted = true;
                                }
                            }

                            if (!hasAlreadyVoted)
                            {
                                answerVotes[answerNumber - 1]++;
                                await e.Channel.SendMessage("A vote has been added to answer[" + answerNumber + "] *" + answers[answerNumber - 1] + "*");
                                await e.Channel.SendMessage(getVoteStats());
                                alreadyVoted.Add(e.User);
                            }
                            else
                            {
                                await e.Channel.SendMessage("You have already voted");
                            }
                            

                        }
                        else
                        {
                            await e.Channel.SendMessage("that's not a valid answer number");
                        }
                    }
                    else
                    {
                        commandDenied(e.Channel);
                    }
                });
                cgb.CreateCommand("end").   //end voting for a poll and clear all variables that need to be cleared
                Do(async (e) =>
                {
                    if (pollCreator == e.Message.User || e.User.HasRole(e.Server.GetRole(236507773687431168)))
                    {
                        pollResults = "";
                        isPollStarted = false;
                        isPollCreated = false;
                        pollResults = "The poll *" + pollQuestion + "* has ended, here are the results:\n\n";
                        for (int i = 0; i < answers.Count; i++)
                        {
                            pollResults += "Answer [" + (i+1) + "]\t*" + answers[i] + "*\twith: " + answerVotes[i] + " votes! \n";
                        }
                        await e.Channel.SendMessage(pollResults);
                        alreadyVoted.Clear();
                        answers.Clear();
                        Array.Clear(answerVotes, 0, answerVotes.Length);
                    }
                    else
                    {
                        commandDenied(e.Channel);
                    }
                });
                cgb.CreateCommand("lastresult").
                Do(async (e) =>
                {
                    await e.Channel.SendMessage(pollResults);
                });
                cgb.CreateCommand("stats").
                Do(async (e) =>
                {
                    await e.Channel.SendMessage(getVoteStats());
                });


            });
            //command group for dalai (specific stuff idk, mostly used to avoid the same command on multiple bots)
            commands.CreateGroup("dalai", cgb =>
            {
                //help command
                cgb.CreateCommand("help")
                    .Do(async e =>
                   {
                   await e.Message.User.SendMessage
                   ("Here is the list of commands that I, Dalai Lama, currently contain.\n" +
                   "1. !dalai help \tThis command will bring up this list.\n" +
                   "2. !confess or !confession \tUse this command to create an anonymous confession, you can send it in any channel and your confession will be deleted and send to #confessions anonymously.\n" +
                   "3. !Compliment @name \tThis command sends a random compliment (out of currently 60 compliments) and mentions the given name. Compliment eachother once in a while!" +
                   "\n**Poll commands**:\n\t1. !poll create *question* \tCreates a new poll with the question behind the command (only 1 poll may be created at a time)\n\t" +
                   "2. !poll answer *answer* \tAssigns an answer to a list of answers where people can vote on\n\t" +
                   "3. !poll start \tThis command starts the poll and enables voting (a poll can only be started with a minimun of 2 answers\n\t" +
                   "4. !poll vote *number of the answer* \tThis command votes on the answer of which the number was given (you can only vote once, you can't re-vote (yet,don't know if this is required))\n\t" +
                   "5. !poll end \tThis command ends the voting for the poll and gives the results, a new poll may be created now\n\t" +
                   "6. !poll lastresult \tThis command gives the results of the last held poll, a poll's results can be recalled untill another poll has ended\n\t" +
                   "This is still kind of an early stage for the poll feature, but all it's functions should work properly. If you find a mistake don't hesitate to let me (@Roefel) know, I will probably add more commands later like !poll stats to give a current overview of the poll's votes. but if you have any ideas for the poll or the bot in general just let me know!"

                       );
                   });
            });

            discord.ExecuteAndWait(async () =>
            {
                await discord.Connect("MjM4NDA0ODE1ODc5NjY3NzEy.Culy0w.vAKQhXwYYuZpa8bd9tIU6-nk3f8", TokenType.Bot);
                discord.SetGame("!dalai help");
            });
        }

        private string compliment()
        {
            int i = rnd.Next(compliments.Length);
            return compliments[i];
        }

        private void addAnswerToList(string answer)
        {
            answers.Add(answer);
            answerPosition++;
        }
        private string getVoteStats()
        {
            voteStats += "poll:\t*" + pollQuestion + "*\n";
            for (int i = 0; i < answers.Count; i++)
            {
                voteStats += "Answer[" + (i + 1) + "]\t *" + answers[i] + "*\t has: " + answerVotes[i] + " votes\n";
            }
            return voteStats;
        }

        private void commandDenied(Channel channel)
        {
            channel.SendMessage("you cannot use that command right now.");
        }

        private void log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
