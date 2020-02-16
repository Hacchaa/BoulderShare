using System.Collections.Generic;
using System.Text;

namespace AdvancedInputFieldPlugin
{
	public class RichTextRegion
	{
		public string content;
		public int startContentPosition;
		public int endContentPosition;
		public List<string> startTags;
		public List<string> endTags;

		public RichTextRegion(string content, int startContentPosition, int endContentPosition, List<string> startTags = null, List<string> endTags = null)
		{
			this.content = content;
			this.startContentPosition = startContentPosition;
			this.endContentPosition = endContentPosition;
			if(startTags != null)
			{
				this.startTags = startTags;
				this.endTags = endTags;
			}
			else
			{
				this.startTags = new List<string>();
				this.endTags = new List<string>();
			}
		}

		public void RemoveTag(string endTag)
		{
			int index = endTags.IndexOf(endTag);
			if(index != -1) //Found tags, so safe to remove
			{
				startTags.RemoveAt(index);
				endTags.RemoveAt(index);
			}
		}

		public void AddTag(string startTag, string endTag)
		{
			int index = endTags.IndexOf(endTag);
			if(index == -1) //No tags found, so safe to add
			{
				startTags.Add(startTag);
				endTags.Add(endTag);
			}
			else
			{
				startTags[index] = startTag; //Replacing start tag, because end tag is the same
			}
		}

		public RichTextRegion[] Split(int index)
		{
			string content1 = content.Substring(0, index);
			string content2 = content.Substring(index);

			RichTextRegion[] regions = new RichTextRegion[2];
			if(startTags != null)
			{
				regions[0] = new RichTextRegion(content1, startContentPosition, startContentPosition + index, new List<string>(startTags), new List<string>(endTags));
				regions[1] = new RichTextRegion(content2, startContentPosition + index, endContentPosition, new List<string>(startTags), new List<string>(endTags));
			}
			else
			{
				regions[0] = new RichTextRegion(content1, startContentPosition, startContentPosition + index, null, null);
				regions[1] = new RichTextRegion(content2, startContentPosition + index, endContentPosition, null, null);
			}

			return regions;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("RichTextRegion: " + content + ": " + startContentPosition + " -> " + endContentPosition);
			int length = startTags.Count;
			for(int i = 0; i < length; i++)
			{
				stringBuilder.AppendLine("Tag: " + startTags[i]);
			}

			return stringBuilder.ToString();
		}
	}
}
