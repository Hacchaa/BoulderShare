using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AdvancedInputFieldPlugin
{
	public class RichTextProcessor
	{
		public RichTextTagInfo[] supportedTags;

		public TextEditFrame LastRichTextEditFrame { get; private set; }
		public TextEditFrame LastTextEditFrame { get; private set; }

		public RichTextProcessor(RichTextTagInfo[] supportedTags)
		{
			this.supportedTags = supportedTags;
		}

		public void SetupRichText(string richText, int richTextCaretPosition)
		{
			LastRichTextEditFrame = new TextEditFrame(richText, richTextCaretPosition, richTextCaretPosition, richTextCaretPosition);

			string text;
			int caretPosition;
			ConfigureRichText(richText, richTextCaretPosition, out text, out caretPosition);

			LastTextEditFrame = new TextEditFrame(text, caretPosition, caretPosition, caretPosition);
		}

		private void ConfigureRichText(string richText, int richTextCaretPosition, out string text, out int caretPosition)
		{
			int tagStartIndex = -1;
			int tagEndIndex = -1;
			caretPosition = -1;
			int offset = 0;
			bool verifyTagForCaret = false;
			StringBuilder stringBuilder = new StringBuilder();

			int length = richText.Length;
			for(int i = 0; i < length; i++)
			{
				if(caretPosition == -1 && i == richTextCaretPosition)
				{
					if(tagStartIndex == -1)
					{
						caretPosition = richTextCaretPosition - offset;
					}
					else
					{
						caretPosition = tagStartIndex - offset;
						verifyTagForCaret = true;
					}
				}

				char c = richText[i];
				if(tagStartIndex == -1)
				{
					if(c == '<')
					{
						tagStartIndex = i;
					}
				}
				else
				{
					if(c == '>')
					{
						string tagText = richText.Substring(tagStartIndex, (i - tagStartIndex) + 1);
						bool isValidTag = IsValidTagText(tagText);

						if(isValidTag)
						{
							offset += tagText.Length;

							int amount = (tagStartIndex - tagEndIndex) - 1;
							if(amount > 0)
							{
								string appendText = richText.Substring(tagEndIndex + 1, amount);
								stringBuilder.Append(appendText);
							}
						}

						if(verifyTagForCaret)
						{
							verifyTagForCaret = false;
							if(!isValidTag) //It wasn't a tag
							{
								caretPosition = richTextCaretPosition - offset;
							}

						}

						tagStartIndex = -1;
						tagEndIndex = i;
					}
					else if(c == '<')
					{
						tagStartIndex = i;
						if(verifyTagForCaret) //It wasn't a tag
						{
							verifyTagForCaret = false;
							caretPosition = richTextCaretPosition - offset;
						}
					}
				}
			}

			if(caretPosition == -1)
			{
				caretPosition = richText.Length - offset;
			}

			if(tagEndIndex != -1 && tagEndIndex + 1 < richText.Length)
			{
				string appendText = richText.Substring(tagEndIndex + 1, (richText.Length - tagEndIndex) - 1);
				stringBuilder.Append(appendText);
			}

			if(tagStartIndex == -1 && tagEndIndex == -1)
			{
				stringBuilder.Append(richText);
			}

			text = stringBuilder.ToString();
		}

		public bool IsValidTagText(string tagText)
		{
			int length = supportedTags.Length;
			for(int i = 0; i < length; i++)
			{
				RichTextTagInfo tagInfo = supportedTags[i];
				switch(tagInfo.type)
				{
					case RichTextTagType.BASIC:
						if(tagInfo.startTag == tagText || tagInfo.endTag == tagText)
						{
							return true;
						}
						break;
					case RichTextTagType.SINGLE_PARAMETER:
						if(tagText.StartsWith(tagInfo.startTagStart) || tagInfo.endTag == tagText)
						{
							return true;
						}
						break;

				}
			}

			return false;
		}

		public bool IsValidStartTagText(string tagText, out int index)
		{
			int length = supportedTags.Length;
			for(int i = 0; i < length; i++)
			{
				RichTextTagInfo tagInfo = supportedTags[i];
				switch(tagInfo.type)
				{
					case RichTextTagType.BASIC:
						if(tagInfo.startTag == tagText)
						{
							index = i;
							return true;
						}
						break;
					case RichTextTagType.SINGLE_PARAMETER:
						if(tagText.StartsWith(tagInfo.startTagStart))
						{
							index = i;
							return true;
						}
						break;

				}
			}

			index = -1;
			return false;
		}

		public bool IsValidEndTagText(string tagText, out int index)
		{
			int length = supportedTags.Length;
			for(int i = 0; i < length; i++)
			{
				RichTextTagInfo tagInfo = supportedTags[i];
				if(tagInfo.endTag == tagText)
				{
					index = i;
					return true;
				}
			}

			index = -1;
			return false;
		}

		/// <summary>Processes a TextEditFrame in rich text to a TextEditFrame in original text</summary>
		public TextEditFrame ProcessRichTextEditFrame(TextEditFrame richTextEditFrame)
		{
			if(richTextEditFrame.selectionStartPosition == richTextEditFrame.selectionEndPosition || richTextEditFrame.selectionStartPosition == -1 || richTextEditFrame.selectionEndPosition == -1) //No selection
			{
				richTextEditFrame.selectionStartPosition = richTextEditFrame.caretPosition;
				richTextEditFrame.selectionEndPosition = richTextEditFrame.caretPosition;
			}

			TextEditFrame textEditFrame = new TextEditFrame();
			if(LastRichTextEditFrame.text == richTextEditFrame.text)
			{
				textEditFrame.text = LastTextEditFrame.text;
				if(LastRichTextEditFrame.caretPosition == richTextEditFrame.caretPosition)
				{
					textEditFrame.caretPosition = LastTextEditFrame.caretPosition;
				}
				else
				{
					textEditFrame.caretPosition = DeterminePositionInText(richTextEditFrame.text, richTextEditFrame.caretPosition);
				}

				if(richTextEditFrame.selectionStartPosition == richTextEditFrame.caretPosition)
				{
					textEditFrame.selectionStartPosition = textEditFrame.caretPosition;
				}
				else if(LastRichTextEditFrame.selectionStartPosition == richTextEditFrame.selectionStartPosition)
				{
					textEditFrame.selectionStartPosition = LastTextEditFrame.selectionStartPosition;
				}
				else
				{
					textEditFrame.selectionStartPosition = DeterminePositionInText(richTextEditFrame.text, richTextEditFrame.selectionStartPosition);
				}

				if(richTextEditFrame.selectionEndPosition == richTextEditFrame.caretPosition)
				{
					textEditFrame.selectionEndPosition = textEditFrame.caretPosition;
				}
				else if(LastRichTextEditFrame.selectionEndPosition == richTextEditFrame.selectionEndPosition)
				{
					textEditFrame.selectionEndPosition = LastTextEditFrame.selectionEndPosition;
				}
				else
				{
					textEditFrame.selectionEndPosition = DeterminePositionInText(richTextEditFrame.text, richTextEditFrame.selectionEndPosition);
				}
			}
			else
			{
				string text;
				int caretPosition;
				ConfigureRichText(richTextEditFrame.text, richTextEditFrame.caretPosition, out text, out caretPosition);

				textEditFrame.text = text;
				textEditFrame.caretPosition = caretPosition;

				if(richTextEditFrame.selectionStartPosition == richTextEditFrame.caretPosition)
				{
					textEditFrame.selectionStartPosition = textEditFrame.caretPosition;
				}
				else
				{
					textEditFrame.selectionStartPosition = DeterminePositionInText(richTextEditFrame.text, richTextEditFrame.selectionStartPosition);
				}

				if(richTextEditFrame.selectionEndPosition == richTextEditFrame.caretPosition)
				{
					textEditFrame.selectionEndPosition = textEditFrame.caretPosition;
				}
				else
				{
					textEditFrame.selectionEndPosition = DeterminePositionInText(richTextEditFrame.text, richTextEditFrame.selectionEndPosition);
				}
			}

			LastTextEditFrame = textEditFrame;
			LastRichTextEditFrame = richTextEditFrame;
			return textEditFrame;
		}

		/// <summary>Processes a TextEditFrame in original text to a TextEditFrame in rich text</summary>
		public TextEditFrame ProcessTextEditFrame(TextEditFrame textEditFrame)
		{
			if(textEditFrame.selectionStartPosition == textEditFrame.selectionEndPosition || textEditFrame.selectionStartPosition == -1 || textEditFrame.selectionEndPosition == -1) //No selection
			{
				textEditFrame.selectionStartPosition = textEditFrame.caretPosition;
				textEditFrame.selectionEndPosition = textEditFrame.caretPosition;
			}

			TextEditFrame richTextEditFrame = new TextEditFrame();
			if(LastTextEditFrame.selectionStartPosition < LastTextEditFrame.selectionEndPosition)
			{
				if(textEditFrame.selectionStartPosition == textEditFrame.selectionEndPosition)
				{
					int previousSelectionAmount = LastTextEditFrame.selectionEndPosition - LastTextEditFrame.selectionStartPosition;
					if(textEditFrame.text.Length > LastTextEditFrame.text.Length - previousSelectionAmount)
					{
						richTextEditFrame = DeleteRichTextNext(LastRichTextEditFrame.text, LastRichTextEditFrame.selectionStartPosition, previousSelectionAmount);
						richTextEditFrame = OptimizeRichTextEditFrame(richTextEditFrame, textEditFrame);

						int insertAmount = textEditFrame.text.Length - (LastTextEditFrame.text.Length - previousSelectionAmount);
						string textToInsert = textEditFrame.text.Substring(LastTextEditFrame.selectionStartPosition, insertAmount);
						int selectionStartPosition = DeterminePositionInRichText(richTextEditFrame.text, LastTextEditFrame.selectionStartPosition);
						richTextEditFrame.text = richTextEditFrame.text.Insert(selectionStartPosition, textToInsert);
						richTextEditFrame.caretPosition = selectionStartPosition + textToInsert.Length;
						richTextEditFrame.selectionStartPosition = richTextEditFrame.caretPosition;
						richTextEditFrame.selectionEndPosition = richTextEditFrame.caretPosition;

					}
					else if(textEditFrame.text.Length == LastTextEditFrame.text.Length - previousSelectionAmount)
					{
						richTextEditFrame = DeleteRichTextNext(LastRichTextEditFrame.text, LastRichTextEditFrame.selectionStartPosition, previousSelectionAmount);
						richTextEditFrame = OptimizeRichTextEditFrame(richTextEditFrame, textEditFrame);
					}
					else
					{
						richTextEditFrame.text = LastTextEditFrame.text;
						richTextEditFrame.caretPosition = LastTextEditFrame.caretPosition;
						richTextEditFrame.selectionStartPosition = LastTextEditFrame.selectionStartPosition;
						richTextEditFrame.selectionEndPosition = LastTextEditFrame.selectionEndPosition;
					}
				}
				else
				{
					richTextEditFrame.text = LastTextEditFrame.text;
					richTextEditFrame.caretPosition = DeterminePositionInText(richTextEditFrame.text, textEditFrame.caretPosition);
					richTextEditFrame.selectionStartPosition = DeterminePositionInText(richTextEditFrame.text, textEditFrame.selectionStartPosition);
					richTextEditFrame.selectionEndPosition = DeterminePositionInText(richTextEditFrame.text, textEditFrame.selectionEndPosition);
				}
			}
			else
			{
				if(textEditFrame.caretPosition < LastTextEditFrame.caretPosition) //Backwards delete
				{
					int amount = LastTextEditFrame.caretPosition - textEditFrame.caretPosition;
					richTextEditFrame = DeleteRichTextPrevious(LastRichTextEditFrame.text, LastRichTextEditFrame.selectionStartPosition, amount);
					richTextEditFrame = OptimizeRichTextEditFrame(richTextEditFrame, textEditFrame);
				}
				else if(textEditFrame.caretPosition > LastTextEditFrame.caretPosition) //Text insert
				{
					string textToInsert = textEditFrame.text.Substring(LastTextEditFrame.caretPosition, textEditFrame.caretPosition - LastTextEditFrame.caretPosition);
					richTextEditFrame.text = LastRichTextEditFrame.text.Insert(LastRichTextEditFrame.caretPosition, textToInsert);
					richTextEditFrame.caretPosition = LastRichTextEditFrame.caretPosition + textToInsert.Length;
					richTextEditFrame.selectionStartPosition = richTextEditFrame.caretPosition;
					richTextEditFrame.selectionEndPosition = richTextEditFrame.caretPosition;
				}
				else //Forward delete
				{
					int amount = LastTextEditFrame.text.Length - textEditFrame.text.Length;
					richTextEditFrame = DeleteRichTextNext(LastRichTextEditFrame.text, LastRichTextEditFrame.selectionStartPosition, amount);
					richTextEditFrame = OptimizeRichTextEditFrame(richTextEditFrame, textEditFrame);
				}
			}

			LastTextEditFrame = textEditFrame;
			LastRichTextEditFrame = richTextEditFrame;
			return richTextEditFrame;
		}

		public TextEditFrame ToggleTagPair(string startTag, string endTag)
		{
			string text = LastRichTextEditFrame.text;

			List<RichTextRegion> regions = ConvertToRegions(text);
			ToggleRichText(regions, startTag, endTag);

			StringBuilder stringBuilder = new StringBuilder();
			OptimizeRichTextRegions(regions, ref stringBuilder);

			string toggledText = stringBuilder.ToString();

			TextEditFrame richTextEditFrame = new TextEditFrame();
			richTextEditFrame.text = toggledText;
			richTextEditFrame.caretPosition = DeterminePositionInRichText(toggledText, LastTextEditFrame.caretPosition);
			richTextEditFrame.selectionStartPosition = DeterminePositionInRichText(toggledText, LastTextEditFrame.selectionStartPosition);
			richTextEditFrame.selectionEndPosition = DeterminePositionInRichText(toggledText, LastTextEditFrame.selectionEndPosition);

			return richTextEditFrame;
		}

		public void ToggleRichText(List<RichTextRegion> regions, string startTagToInsert, string endTagToInsert)
		{
			int start = LastRichTextEditFrame.selectionStartPosition;
			int end = LastRichTextEditFrame.selectionEndPosition;
			bool startFound = false;
			bool endFound = false;
			bool toggleON = false;

			int length = regions.Count;
			for(int i = 0; i < length; i++)
			{
				RichTextRegion region = regions[i];
				if(!startFound && ((start >= region.startContentPosition && start <= region.endContentPosition) || start <= region.startContentPosition))
				{
					if(start <= region.startContentPosition)
					{
						start = region.startContentPosition;
					}

					startFound = true;

					if(region.endTags != null && region.endTags.Contains(endTagToInsert))
					{
						if(region.startTags.Contains(startTagToInsert)) //Has same tag pair, so toggle off
						{
							toggleON = false;
						}
						else //Has same tag pair, but with different parameter, so update parameter
						{
							toggleON = true;
						}
					}
					else
					{
						toggleON = true;
					}

					if(start == region.startContentPosition)
					{
						if(end >= region.endContentPosition)
						{
							if(toggleON)
							{
								region.AddTag(startTagToInsert, endTagToInsert);
							}
							else
							{
								region.RemoveTag(endTagToInsert);
							}
							i--; //Make sure to recheck for selection end
						}
						else if(end <= region.endContentPosition)
						{
							endFound = true;
							RichTextRegion[] splitRegions = region.Split(end - start);
							regions.RemoveAt(i);
							regions.Insert(i, splitRegions[0]);
							regions.Insert(i + 1, splitRegions[1]);
							length++;

							if(toggleON)
							{
								splitRegions[0].AddTag(startTagToInsert, endTagToInsert);
							}
							else
							{
								splitRegions[0].RemoveTag(endTagToInsert);
							}
						}
					}
					else
					{
						RichTextRegion[] splitRegions = region.Split(start - region.startContentPosition);
						regions.RemoveAt(i);
						regions.Insert(i, splitRegions[0]);

						if(end <= region.endContentPosition)
						{
							endFound = true;
							RichTextRegion[] splitRegions2 = splitRegions[1].Split(end - start);
							regions.Insert(i + 1, splitRegions2[0]);
							regions.Insert(i + 2, splitRegions2[1]);
							length += 2;

							if(toggleON)
							{
								splitRegions2[0].AddTag(startTagToInsert, endTagToInsert);
							}
							else
							{
								splitRegions2[0].RemoveTag(endTagToInsert);
							}
						}
						else
						{
							regions.Insert(i + 1, splitRegions[1]);
							length++;

							if(toggleON)
							{
								splitRegions[1].AddTag(startTagToInsert, endTagToInsert);
							}
							else
							{
								splitRegions[1].RemoveTag(endTagToInsert);
							}
						}
					}
				}
				else if(!endFound && end >= region.startContentPosition && end <= region.endContentPosition + 1)
				{
					endFound = true;

					if(end == region.startContentPosition)
					{
						continue;
					}
					else if(end == region.endContentPosition + 1 || ((end - region.startContentPosition) == region.content.Length))
					{
						if(toggleON)
						{
							region.AddTag(startTagToInsert, endTagToInsert);
						}
						else
						{
							region.RemoveTag(endTagToInsert);
						}
					}
					else
					{
						RichTextRegion[] splitRegions = region.Split(end - region.startContentPosition);
						regions.RemoveAt(i);
						regions.Insert(i, splitRegions[0]);
						regions.Insert(i + 1, splitRegions[1]);
						length++;

						if(toggleON)
						{
							splitRegions[0].AddTag(startTagToInsert, endTagToInsert);
						}
						else
						{
							splitRegions[0].RemoveTag(endTagToInsert);
						}
					}
				}
				else if(startFound && !endFound)
				{
					if(toggleON)
					{
						region.AddTag(startTagToInsert, endTagToInsert);
					}
					else
					{
						region.RemoveTag(endTagToInsert);
					}
				}
			}
		}

		public TextEditFrame OptimizeRichTextEditFrame(TextEditFrame richTextEditFrame, TextEditFrame textEditFrame)
		{
			if(string.IsNullOrEmpty(richTextEditFrame.text))
			{
				richTextEditFrame.text = string.Empty;
				richTextEditFrame.caretPosition = 0;
				richTextEditFrame.selectionStartPosition = 0;
				richTextEditFrame.selectionEndPosition = 0;
				return richTextEditFrame;
			}

			List<RichTextRegion> regions = ConvertToRegions(richTextEditFrame.text);
			StringBuilder stringBuilder = new StringBuilder();
			OptimizeRichTextRegions(regions, ref stringBuilder);
			richTextEditFrame.text = stringBuilder.ToString();
			richTextEditFrame.caretPosition = DeterminePositionInRichText(richTextEditFrame.text, textEditFrame.caretPosition);
			if(textEditFrame.selectionStartPosition == textEditFrame.caretPosition)
			{
				richTextEditFrame.selectionStartPosition = richTextEditFrame.caretPosition;
			}
			else
			{
				richTextEditFrame.selectionStartPosition = DeterminePositionInText(richTextEditFrame.text, textEditFrame.selectionStartPosition);
			}

			if(textEditFrame.selectionEndPosition == textEditFrame.caretPosition)
			{
				richTextEditFrame.selectionEndPosition = richTextEditFrame.caretPosition;
			}
			else if(textEditFrame.selectionEndPosition == textEditFrame.selectionStartPosition)
			{
				richTextEditFrame.selectionEndPosition = richTextEditFrame.selectionStartPosition;
			}
			else
			{
				richTextEditFrame.selectionEndPosition = DeterminePositionInText(richTextEditFrame.text, textEditFrame.selectionEndPosition);
			}

			return richTextEditFrame;
		}

		public TextEditFrame DeleteRichTextPrevious(string richText, int richTextPosition, int amount)
		{
			int previousIndex = -1;
			int endTagIndex = -1;
			bool skippedCharacter = false;
			int amountDeleted = 0;

			int length = richText.Length;
			for(int i = richTextPosition; i >= 0; i--)
			{
				if(i == length)
				{
					skippedCharacter = true;
					continue;
				}

				if(richText[i] == '>')
				{
					if(endTagIndex != -1) //This one was not a tag
					{
						richText = richText.Remove(endTagIndex, 1);
						length--;
						amountDeleted++;
						if(amountDeleted == amount)
						{
							break;
						}

						i = endTagIndex + 1; //Recheck
						previousIndex = -1;
						endTagIndex = -1;
					}
					else
					{
						endTagIndex = i;
					}
				}
				else if(i == richTextPosition)
				{
					skippedCharacter = true;
					continue;
				}
				else if(endTagIndex == -1)
				{
					if(previousIndex == -1)
					{
						if(skippedCharacter)
						{
							previousIndex = Mathf.Max(0, i);
						}
						else
						{
							previousIndex = Mathf.Max(0, i - 1);
						}
					}

					richText = richText.Remove(previousIndex, 1);
					length--;
					amountDeleted++;
					if(amountDeleted == amount)
					{
						break;
					}

					i++; //Recheck
					previousIndex = -1;
				}
				else if(richText[i] == '<')
				{
					string tagText = richText.Substring(i, (endTagIndex - i) + 1);
					bool isTag = IsValidTagText(tagText);

					if(isTag)
					{
						previousIndex = Mathf.Max(0, i - 1);
						endTagIndex = -1;
					}
					else
					{
						richText = richText.Remove(endTagIndex, 1);
						length--;
						amountDeleted++;
						if(amountDeleted == amount)
						{
							break;
						}

						i = endTagIndex + 1; //Recheck
						previousIndex = -1;
						endTagIndex = -1;
					}
				}

				if(i == 0 && endTagIndex != -1) //This can't be a tag at this point, just delete
				{
					richText = richText.Remove(endTagIndex, 1);
					length--;
					amountDeleted++;
					if(amountDeleted == amount)
					{
						break;
					}

					i = endTagIndex + 1; //Recheck
					previousIndex = -1;
					endTagIndex = -1;
				}
			}

			return new TextEditFrame(richText, previousIndex, previousIndex, previousIndex);
		}

		public int GetRichTextPrevious()
		{
			string richText = LastRichTextEditFrame.text;
			int richTextPosition = LastRichTextEditFrame.caretPosition;
			int previousIndex = -1;
			int endTagIndex = -1;
			bool skippedCharacter = false;

			int length = richText.Length;
			for(int i = richTextPosition; i >= 0; i--)
			{
				if(i == length)
				{
					skippedCharacter = true;
					continue;
				}

				if(richText[i] == '>')
				{
					if(endTagIndex != -1) //This one was not a tag
					{
						return endTagIndex;
					}
					else
					{
						endTagIndex = i;
					}
				}
				else if(i == richTextPosition)
				{
					skippedCharacter = true;
					continue;
				}
				else if(endTagIndex == -1)
				{
					if(previousIndex == -1)
					{
						if(skippedCharacter)
						{
							previousIndex = Mathf.Max(0, i);
						}
						else
						{
							previousIndex = Mathf.Max(0, i - 1);
						}
					}

					return previousIndex;
				}
				else if(richText[i] == '<')
				{
					string tagText = richText.Substring(i, (endTagIndex - i) + 1);
					bool isTag = IsValidTagText(tagText);

					if(isTag)
					{
						previousIndex = Mathf.Max(0, i - 1);
						endTagIndex = -1;
					}
					else
					{
						return endTagIndex;
					}
				}

				if(i == 0 && endTagIndex != -1) //This can't be a tag at this point
				{
					return endTagIndex;
				}
			}

			return -1;
		}

		public TextEditFrame DeleteRichTextNext(string richText, int richTextPosition, int amount)
		{
			int previousIndex = -1;
			int startTagIndex = -1;
			int amountDeleted = 0;

			int length = richText.Length;
			for(int i = richTextPosition; i < length; i++)
			{
				if(richText[i] == '<')
				{
					if(startTagIndex != -1) //This one was not a tag
					{
						richText = richText.Remove(startTagIndex, 1);
						length--;
						amountDeleted++;
						if(amountDeleted == amount)
						{
							break;
						}

						i = startTagIndex - 1; //Recheck
						previousIndex = -1;
						startTagIndex = -1;
					}
					else
					{
						startTagIndex = i;
					}
				}
				else if(startTagIndex == -1)
				{
					if(previousIndex == -1)
					{
						previousIndex = Mathf.Min(richText.Length - 1, i);
					}

					richText = richText.Remove(previousIndex, 1);
					length--;
					amountDeleted++;
					if(amountDeleted == amount)
					{
						break;
					}

					i--; //Recheck
					previousIndex = -1;
				}
				else if(richText[i] == '>')
				{
					string tagText = richText.Substring(startTagIndex, (i - startTagIndex) + 1);
					bool isTag = IsValidTagText(tagText);

					if(isTag)
					{
						previousIndex = -1;
						startTagIndex = -1;
					}
					else
					{
						richText = richText.Remove(startTagIndex, 1);
						length--;
						amountDeleted++;
						if(amountDeleted == amount)
						{
							break;
						}

						i = startTagIndex - 1; //Recheck
						previousIndex = -1;
						startTagIndex = -1;
					}
				}

				if(i == length - 1 && startTagIndex != -1) //This can't be a tag at this point, just delete
				{
					richText = richText.Remove(startTagIndex, 1);
					length--;
					amountDeleted++;
					if(amountDeleted == amount)
					{
						break;
					}

					i = startTagIndex - 1; //Recheck
					previousIndex = -1;
					startTagIndex = -1;
				}
			}

			return new TextEditFrame(richText, previousIndex, previousIndex, previousIndex);
		}

		public int GetRichTextNext()
		{
			string richText = LastRichTextEditFrame.text;
			int richTextPosition = LastRichTextEditFrame.caretPosition;
			int previousIndex = -1;
			int startTagIndex = -1;

			int length = richText.Length;
			for(int i = richTextPosition; i < length; i++)
			{
				if(richText[i] == '<')
				{
					if(startTagIndex != -1) //This one was not a tag
					{
						return startTagIndex + 1;
					}
					else
					{
						startTagIndex = i;
					}
				}
				else if(startTagIndex == -1)
				{
					if(previousIndex == -1)
					{
						previousIndex = Mathf.Min(richText.Length - 1, i);
					}

					return previousIndex + 1;
				}
				else if(richText[i] == '>')
				{
					string tagText = richText.Substring(startTagIndex, (i - startTagIndex) + 1);
					bool isTag = IsValidTagText(tagText);

					if(isTag)
					{
						previousIndex = -1;
						startTagIndex = -1;
					}
					else
					{
						return startTagIndex + 1;
					}
				}

				if(i == length - 1 && startTagIndex != -1) //This can't be a tag at this point, just delete
				{
					return startTagIndex + 1;
				}
			}

			return -1;
		}

		public int DeterminePositionInText(string richText, int richTextPosition)
		{
			int startTagIndex = -1;
			int offset = 0;

			int length = richText.Length;
			for(int i = 0; i < length; i++)
			{
				if(i == richTextPosition)
				{
					return (richTextPosition - offset);
				}

				if(richText[i] == '<')
				{
					startTagIndex = i;
				}
				else if(startTagIndex != -1 && richText[i] == '>')
				{
					string tagText = richText.Substring(startTagIndex, (i - startTagIndex) + 1);
					bool isTag = IsValidTagText(tagText);

					if(isTag)
					{
						offset += tagText.Length;
						startTagIndex = -1;
					}
				}
			}

			return (length - offset);
		}

		public int DeterminePositionInRichText(string richText, int textPosition)
		{
			int startTagIndex = -1;
			int offset = 0;

			int length = richText.Length;
			for(int i = 0; i < length; i++)
			{
				if(richText[i] == '<')
				{
					if(startTagIndex != -1 && i > textPosition + offset) //This was not a tag
					{
						return (textPosition + offset);
					}
					startTagIndex = i;
				}
				else if(startTagIndex != -1)
				{
					if(richText[i] == '>')
					{
						string tagText = richText.Substring(startTagIndex, (i - startTagIndex) + 1);
						bool isTag = IsValidTagText(tagText);

						if(isTag)
						{
							offset += tagText.Length;
							startTagIndex = -1;
						}
						else if(i >= textPosition + offset)
						{
							return (textPosition + offset);
						}
					}
				}
				else if(i >= textPosition + offset)
				{
					return i;
				}
			}

			return length;
		}

		public List<RichTextRegion> ConvertToRegions(string text)
		{
			List<RichTextRegion> regions = new List<RichTextRegion>();
			if(string.IsNullOrEmpty(text)) { return regions; }

			Stack<string> startTagsStack = new Stack<string>();
			Stack<string> endTagsStack = new Stack<string>();
			int tagStartIndex = -1;
			int tagEndIndex = -1;

			int length = text.Length;
			for(int i = 0; i < length; i++)
			{
				char c = text[i];
				if(c == '<')
				{
					tagStartIndex = i;
				}
				else if(c == '>' && tagStartIndex != -1)
				{
					string tagText = text.Substring(tagStartIndex, (i - tagStartIndex) + 1);

					int si = 0;
					bool isStartTag = IsValidStartTagText(tagText, out si);
					if(isStartTag)
					{
						if(tagEndIndex == -1)
						{
							string content = text.Substring(0, tagStartIndex);
							regions.Add(new RichTextRegion(content, 0, tagStartIndex - 1));
						}
						else if(tagEndIndex + 1 != tagStartIndex)
						{
							int amount = tagStartIndex - (tagEndIndex + 1);
							string content = text.Substring(tagEndIndex + 1, amount);
							regions.Add(new RichTextRegion(content, tagEndIndex + 1, tagEndIndex + amount, new List<string>(startTagsStack), new List<string>(endTagsStack)));
						}

						startTagsStack.Push(tagText); //Use tag text here to preserve the parameter value
						endTagsStack.Push(supportedTags[si].endTag);

						tagStartIndex = -1;
						tagEndIndex = i;
						continue;
					}

					int ei = 0;
					bool isEndTag = IsValidEndTagText(tagText, out ei);
					if(isEndTag)
					{
						if(tagEndIndex + 1 != tagStartIndex)
						{
							int amount = tagStartIndex - (tagEndIndex + 1);
							string content = text.Substring(tagEndIndex + 1, amount);
							regions.Add(new RichTextRegion(content, tagEndIndex + 1, tagEndIndex + amount, new List<string>(startTagsStack), new List<string>(endTagsStack)));
						}

						startTagsStack.Pop();
						endTagsStack.Pop();

						tagStartIndex = -1;
						tagEndIndex = i;
						continue;
					}
				}
			}

			if(tagEndIndex != -1)
			{
				if(tagEndIndex < length - 1)
				{
					int amount = length - (tagEndIndex + 1);
					string content = text.Substring(tagEndIndex + 1, amount);
					regions.Add(new RichTextRegion(content, tagEndIndex + 1, tagEndIndex + amount, null, null));
				}
			}
			else
			{
				regions.Add(new RichTextRegion(text, 0, length - 1, null, null));
			}

			return regions;
		}

		public void OptimizeRichTextRegions(List<RichTextRegion> regions, ref StringBuilder stringBuilder)
		{
			string optimalStartTag = null;
			string optimalEndTag = null;
			int lastRegionIndex = -1;
			int optimalMergeAmount = 0;

			int length = regions.Count;
			for(int i = 0; i < length; i++)
			{
				RichTextRegion region = regions[i];
				List<string> startTags = region.startTags;

				if(i == length - 1) //Last region, just add it and be done
				{
					List<string> endTags = region.endTags;

					int tagsLength = startTags.Count;
					for(int ti = 0; ti < tagsLength; ti++)
					{
						stringBuilder.Append(startTags[ti]);
					}

					stringBuilder.Append(region.content);

					for(int ti = 0; ti < tagsLength; ti++)
					{
						stringBuilder.Append(endTags[tagsLength - 1 - ti]);
					}

					break;
				}

				int startTagsLength = startTags.Count;
				for(int si = 0; si < startTagsLength; si++)
				{
					string startTag = startTags[si];
					int mergeAmount = 0;

					for(int ri = i + 1; ri < length; ri++)
					{
						RichTextRegion otherRegion = regions[ri];
						if(otherRegion.startTags.Contains(startTag))
						{
							mergeAmount++;
							if(mergeAmount > optimalMergeAmount)
							{
								optimalStartTag = startTag;
								optimalEndTag = region.endTags[si];
								optimalMergeAmount = mergeAmount;
								lastRegionIndex = ri;
							}
						}
						else
						{
							break;
						}
					}
				}

				if(optimalMergeAmount > 0)
				{
					stringBuilder.Append(optimalStartTag);

					for(int ri = i; ri <= lastRegionIndex; ri++)
					{
						RichTextRegion mergeRegion = regions[ri];

						List<string> mergeStartTags = mergeRegion.startTags;
						List<string> mergeEndTags = mergeRegion.endTags;

						int tagsLength = mergeStartTags.Count;
						for(int ti = 0; ti < tagsLength; ti++)
						{
							string startTag = mergeStartTags[ti];
							if(startTag != optimalStartTag)
							{
								stringBuilder.Append(startTag);
							}
						}

						stringBuilder.Append(mergeRegion.content);

						for(int ti = 0; ti < tagsLength; ti++)
						{
							string endTag = mergeEndTags[tagsLength - 1 - ti];
							if(endTag != optimalEndTag)
							{
								stringBuilder.Append(endTag);
							}
						}
					}

					stringBuilder.Append(optimalEndTag);

					i = lastRegionIndex;
					optimalStartTag = null;
					optimalEndTag = null;
					lastRegionIndex = -1;
					optimalMergeAmount = 0;
				}
				else
				{
					List<string> endTags = region.endTags;

					int tagsLength = startTags.Count;
					for(int ti = 0; ti < tagsLength; ti++)
					{
						stringBuilder.Append(startTags[ti]);
					}

					stringBuilder.Append(region.content);

					for(int ti = 0; ti < tagsLength; ti++)
					{
						stringBuilder.Append(endTags[tagsLength - 1 - ti]);
					}
				}
			}
		}
	}
}
