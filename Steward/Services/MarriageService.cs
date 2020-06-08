using Discord.WebSocket;
using Steward.Context;
using Steward.Context.Models;
using System;
using System.Web;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Microsoft.EntityFrameworkCore;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;

namespace Steward.Services
{
    public class MarriageService
    {
        private readonly DiscordSocketClient _client;

        private readonly StewardContext _stewardContext;

        public MarriageService(StewardContext stewardContext, DiscordSocketClient client)
        {
            _client = client;
            _stewardContext = stewardContext;
        }

        public async Task SendMarriageMessage(Proposal proposal)
        {
            var proposerID = proposal.Proposer.DiscordId;
            var proposedID = proposal.Proposed.DiscordId;
            var proposer = _client.GetUser(ulong.Parse(proposerID));
            var proposed = _client.GetUser(ulong.Parse(proposedID));
            var marriageChannels = _stewardContext.MarriageChannels.ToList();
            var ProposerChar = proposal.Proposer.Characters.FirstOrDefault(c => c.IsAlive());

            var ProposedChar = proposal.Proposed.Characters.FirstOrDefault(c => c.IsAlive());

            var weddingphoto = CreateWeddingPhoto(GetAvatarImage(proposer), GetAvatarImage(proposed));
            

            var embedBuilder = new EmbedBuilder()
            {
                Title = "A new noble marriage has happend!"
            };
            embedBuilder.AddField( new EmbedFieldBuilder()
            {
                Name = "The spouses are:",
                Value = $"**{ProposerChar.CharacterName}**({proposer.ToString()}) married **{ProposedChar.CharacterName}**({proposed.ToString()})",
                IsInline = false
            });
            string fileName = "weddingphoto.png";
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Steward\data\", fileName);
            weddingphoto.Save(path);

            foreach (var marriageChannel in marriageChannels.Select(marriage => _client.GetChannel(ulong.Parse(marriage.ChannelId)) as SocketTextChannel))
            {
                try
                {
                    //await marriageChannel.SendMessageAsync(embed: embedBuilder.Build());
                    //if the embed does not work
                    await marriageChannel.SendFileAsync(path, "",false,embedBuilder.Build());
                    
                }
                catch (NullReferenceException e)
                {
                    Console.WriteLine(e.StackTrace);
                    //nothing, I just don't want it to crash the command
                }
            }
        }

        public System.Drawing.Image GetAvatarImage(SocketUser user)
        {
            System.Drawing.Image image = null;



            try
            {
                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(user.GetAvatarUrl());
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.Timeout = 30000;

                System.Net.WebResponse webResponse = webRequest.GetResponse();

                System.IO.Stream stream = webResponse.GetResponseStream();

                image = System.Drawing.Image.FromStream(stream);

                webResponse.Close();
            }
            catch (Exception ex)
            {
                return null;
            }

            return image;
        }

        public System.Drawing.Image CreateWeddingPhoto(System.Drawing.Image spouse1, System.Drawing.Image spouse2)
        {
            var spouse1Img = new ImageFactory(false);
            spouse1Img.Load(spouse1);
            spouse1Img.Resize(new Size(500,500));
            var spouse2Img = new ImageFactory(false);
            spouse2Img.Load(spouse2);
            spouse2Img.Resize(new Size(500, 500));

            string fileName = "heart.png";
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Steward\data\", fileName);

            var heart = new ImageFactory(false);
            heart.Load(path);

            int outputImageWidth = spouse1Img.Image.Width + spouse2Img.Image.Width + heart.Image.Width;

            int outputImageHeight = 500;

            Bitmap outputImage = new Bitmap(outputImageWidth, outputImageHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(outputImage))
            {
                graphics.DrawImage(spouse1Img.Image, new Rectangle(new Point(), spouse1Img.Image.Size),
                    new Rectangle(new Point(), spouse1Img.Image.Size), GraphicsUnit.Pixel);
                graphics.DrawImage(heart.Image, new Rectangle(new Point(spouse1Img.Image.Width + 1, 0), heart.Image.Size),
                    new Rectangle(new Point(), heart.Image.Size), GraphicsUnit.Pixel);
                graphics.DrawImage(spouse2Img.Image, new Rectangle(new Point(spouse1Img.Image.Width + heart.Image.Width + 1, 0), spouse2Img.Image.Size),
                    new Rectangle(new Point(), heart.Image.Size), GraphicsUnit.Pixel);
            }

            return outputImage;
        }

        public System.Drawing.Image CreateDivorcePhoto(System.Drawing.Image spouse1, System.Drawing.Image spouse2)
        {
            var spouse1Img = new ImageFactory(false);
            spouse1Img.Load(spouse1);
            spouse1Img.Resize(new Size(500, 500));
            var spouse2Img = new ImageFactory(false);
            spouse2Img.Load(spouse2);
            spouse2Img.Resize(new Size(500, 500));

            string fileName = "brokenheart.png";
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Steward\data\", fileName);

            var heart = new ImageFactory(false);
            heart.Load(path);

            int outputImageWidth = spouse1Img.Image.Width + spouse2Img.Image.Width + heart.Image.Width;

            int outputImageHeight = 500;

            Bitmap outputImage = new Bitmap(outputImageWidth, outputImageHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(outputImage))
            {
                graphics.DrawImage(spouse1Img.Image, new Rectangle(new Point(), spouse1Img.Image.Size),
                    new Rectangle(new Point(), spouse1Img.Image.Size), GraphicsUnit.Pixel);
                graphics.DrawImage(heart.Image, new Rectangle(new Point(spouse1Img.Image.Width + 1, 0), heart.Image.Size),
                    new Rectangle(new Point(), heart.Image.Size), GraphicsUnit.Pixel);
                graphics.DrawImage(spouse2Img.Image, new Rectangle(new Point(spouse1Img.Image.Width + heart.Image.Width + 1, 0), spouse2Img.Image.Size),
                    new Rectangle(new Point(), heart.Image.Size), GraphicsUnit.Pixel);
            }

            return outputImage;
        }

        public async Task SendDivorceMessage(DiscordUser proposerUser, DiscordUser proposedUser)
        {
            var proposerID = proposerUser.DiscordId;
            var proposedID = proposedUser.DiscordId;
            var proposer = _client.GetUser(ulong.Parse(proposerID));
            var proposed = _client.GetUser(ulong.Parse(proposedID));
            var marriageChannels = _stewardContext.MarriageChannels.ToList();
            var ProposerChar = proposerUser.Characters.FirstOrDefault(c => c.IsAlive());

            var ProposedChar = proposedUser.Characters.FirstOrDefault(c => c.IsAlive());

            var DivorcePhoto = CreateDivorcePhoto(GetAvatarImage(proposer), GetAvatarImage(proposed));

            var embedBuilder = new EmbedBuilder()
            {
                Title = "Two lovers have decided to part ways!"
            };
            embedBuilder.AddField(new EmbedFieldBuilder()
            {
                Name = "The divorcees are:",
                Value = $"**{ProposerChar.CharacterName}**({proposer.ToString()}) married **{ProposedChar.CharacterName}**({proposed.ToString()})",
                IsInline = false
            });
            string fileName = "weddingphoto.png";
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Steward\data\", fileName);
            DivorcePhoto.Save(path);

            foreach (var marriageChannel in marriageChannels.Select(marriage => _client.GetChannel(ulong.Parse(marriage.ChannelId)) as SocketTextChannel))
            {
                try
                {
                    //await marriageChannel.SendMessageAsync(embed: embedBuilder.Build());
                    //if the embed does not work
                    await marriageChannel.SendFileAsync(path, "", false, embedBuilder.Build());

                }
                catch (NullReferenceException e)
                {
                    Console.WriteLine(e.StackTrace);
                    //nothing, I just don't want it to crash the command
                }
            }
        }
    }
}
