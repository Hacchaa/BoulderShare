using UnityEngine;

namespace AdvancedInputFieldPlugin
{
	/// <summary>The Rich Text Tag</summary>
	public enum RichTextTag
	{
		//Basic
		BOLD,
		ITALIC,
		LOWERCASE,
		NON_BREAKING_SPACES,
		NOPARSE,
		STRIKETHROUGH,
		SMALLCAPS,
		SUBSCRIPT,
		SUPERSCRIPT,
		UNDERLINE,
		UPPERCASE,
		//Single Parameter
		ALIGN,
		ALPHA,
		COLOR,
		CHARACTER_SPACE,
		FONT,
		INDENT,
		LINE_HEIGHT,
		LINE_INDENT,
		LINK,
		MARGIN,
		MARK,
		MATERIAL,
		MONOSPACE,
		POSITION,
		SIZE,
		STYLE,
		VERTICAL_OFFSET,
		WIDTH,
	}

	public class RichTextData: ScriptableObject
	{
		[Tooltip("The Rich Text tags you want to support")]
		[SerializeField]
		private RichTextTag[] tags;

		public RichTextTagInfo[] GetSupportedTags()
		{
			RichTextTagInfo[] supportedTags = new RichTextTagInfo[tags.Length];

			int length = tags.Length;
			for(int i = 0; i < length; i++)
			{
				RichTextTag tag = tags[i];
				switch(tag)
				{
					case RichTextTag.BOLD:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.BASIC, "<b>", "</b>");
						break;
					case RichTextTag.ITALIC:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.BASIC, "<i>", "</i>");
						break;
					case RichTextTag.LOWERCASE:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.BASIC, "<lowercase>", "</lowercase>");
						break;
					case RichTextTag.NON_BREAKING_SPACES:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.BASIC, "<nobr>", "</nobr>");
						break;
					case RichTextTag.NOPARSE:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.BASIC, "<noparse>", "</noparse>");
						break;
					case RichTextTag.STRIKETHROUGH:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.BASIC, "<s>", "</s>");
						break;
					case RichTextTag.SMALLCAPS:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.BASIC, "<smallcaps>", "</smallcaps>");
						break;
					case RichTextTag.SUBSCRIPT:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.BASIC, "<sub>", "</sub>");
						break;
					case RichTextTag.SUPERSCRIPT:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.BASIC, "<sup>", "</sup>");
						break;
					case RichTextTag.UNDERLINE:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.BASIC, "<u>", "</u>");
						break;
					case RichTextTag.UPPERCASE:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.BASIC, "<uppercase>", "</uppercase>");
						break;
					case RichTextTag.ALIGN:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.SINGLE_PARAMETER, "<align={0}>", "</align>");
						break;
					case RichTextTag.ALPHA:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.SINGLE_PARAMETER, "<alpha={0}>", "</alpha>");
						break;
					case RichTextTag.COLOR:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.SINGLE_PARAMETER, "<color={0}>", "</color>");
						break;
					case RichTextTag.CHARACTER_SPACE:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.SINGLE_PARAMETER, "<cspace={0}>", "</cspace>");
						break;
					case RichTextTag.FONT:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.SINGLE_PARAMETER, "<font={0}>", "</font>");
						break;
					case RichTextTag.INDENT:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.SINGLE_PARAMETER, "<indent={0}>", "</indent>");
						break;
					case RichTextTag.LINE_HEIGHT:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.SINGLE_PARAMETER, "<line-height={0}>", "</line-height>");
						break;
					case RichTextTag.LINE_INDENT:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.SINGLE_PARAMETER, "<line-indent={0}>", "</line-indent>");
						break;
					case RichTextTag.LINK:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.SINGLE_PARAMETER, "<link={0}>", "</link>");
						break;
					case RichTextTag.MARGIN:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.SINGLE_PARAMETER, "<margin={0}>", "</margin>");
						break;
					case RichTextTag.MARK:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.SINGLE_PARAMETER, "<mark={0}>", "</mark>");
						break;
					case RichTextTag.MATERIAL:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.SINGLE_PARAMETER, "<material={0}>", "</material>");
						break;
					case RichTextTag.MONOSPACE:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.SINGLE_PARAMETER, "<mspace={0}>", "</mspace>");
						break;
					case RichTextTag.POSITION:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.SINGLE_PARAMETER, "<pos={0}>", "</pos>");
						break;
					case RichTextTag.SIZE:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.SINGLE_PARAMETER, "<size={0}>", "</size>");
						break;
					case RichTextTag.STYLE:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.SINGLE_PARAMETER, "<style={0}>", "</style>");
						break;
					case RichTextTag.VERTICAL_OFFSET:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.SINGLE_PARAMETER, "<voffset={0}>", "</voffset>");
						break;
					case RichTextTag.WIDTH:
						supportedTags[i] = new RichTextTagInfo(RichTextTagType.SINGLE_PARAMETER, "<width={0}>", "</width>");
						break;
				}
			}

			return supportedTags;
		}
	}
}