﻿using ActionTimeline;
using ActionTimeline.Helpers;
using ActionTimelineEx.Configurations;
using ImGuiNET;
using ImGuiScene;
using System.Drawing;
using System.Numerics;

namespace ActionTimelineEx.Timeline;

public class StatusLineItem : ITimelineItem
{
    public uint Icon { get; set; }
    public float TimeDuration { get; set; }
    public DateTime StartTime { get; init; }

    public DateTime EndTime => StartTime + TimeSpan.FromSeconds(TimeDuration);

    public byte Stack { get; set; }

    public void Draw(DateTime time, Vector2 windowPos, Vector2 windowSize, DrawingSettings setting)
    {
        var rightCenter = windowPos + (setting.IsHorizonal
            ? new Vector2(windowSize.X, windowSize.Y / 2 + setting.CenterOffset)
            : new Vector2(windowSize.X / 2 + setting.CenterOffset, windowSize.Y));
        rightCenter -= setting.TimeOffset * setting.TimeDirectionPerSecond;
        DrawItemWithCenter(rightCenter - (float)(time - StartTime).TotalSeconds * setting.TimeDirectionPerSecond, windowPos, setting);
    }

    public void DrawItemWithCenter(Vector2 centerPos, Vector2 windowPos, DrawingSettings setting)
    {
        var GcdSize = setting.GCDIconSize;
        var drawList = ImGui.GetWindowDrawList();

        var statusHeight = setting.StatusLineSize;
        var flag = ImDrawFlags.RoundCornersAll;
        var rounding = setting.GCDRound;

        TextureWrap? texture = DrawHelper.GetTextureFromIconId(Icon);
        if (texture == null) return;

        var col = DrawHelper.GetTextureAverageColor(Icon);

        var leftTop = centerPos + setting.DownDirection * (statusHeight * (setting.IsReverse ? Stack + 1 : Stack) + GcdSize / 2);
        if(setting.ShowAutoAttack)
        {
            var autoAttackOffset = setting.AutoAttackOffset;
            var autoAttackSize = setting.AutoAttackIconSize;
            leftTop += setting.DownDirection * (autoAttackOffset * GcdSize + autoAttackSize);
        }
        var statusWidth = setting.IsHorizonal ? statusHeight : statusHeight / TimelineItem.HeightRatio;
        var shrink = statusWidth * 0.3f * setting.RealDownDirection;
        var rightBottom = leftTop + setting.TimeDirectionPerSecond * TimeDuration + setting.RealDownDirection * statusWidth - shrink;

        drawList.AddRectFilled(leftTop + shrink, rightBottom,col, rounding, flag);

        if (rightBottom.X <= windowPos.X) return;

        leftTop.X = Math.Max(leftTop.X, windowPos.X);
        leftTop.Y = Math.Max(leftTop.Y, windowPos.Y);

        drawList.AddImage(texture.ImGuiHandle, leftTop,
            leftTop + new Vector2(statusHeight / TimelineItem.HeightRatio, statusHeight), Vector2.Zero, Vector2.One);
    }
}
