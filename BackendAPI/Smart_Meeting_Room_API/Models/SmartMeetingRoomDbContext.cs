using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Smart_Meeting_Room_API.Models;

public partial class SmartMeetingRoomDbContext : DbContext
{

    public SmartMeetingRoomDbContext(DbContextOptions<SmartMeetingRoomDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActionItem> ActionItems { get; set; }

    public virtual DbSet<Attachment> Attachments { get; set; }

    public virtual DbSet<Feature> Features { get; set; }

    public virtual DbSet<Meeting> Meetings { get; set; }

    public virtual DbSet<Mom> Moms { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActionItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ActionIt__3214EC07ECA1F881");

            entity.HasIndex(e => e.AssignedTo, "IX_ActionItems_assignedTo");

            entity.HasIndex(e => e.MomId, "IX_ActionItems_momId");

            entity.Property(e => e.AssignedTo).HasColumnName("assignedTo");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.DueDate).HasColumnName("dueDate");
            entity.Property(e => e.MomId).HasColumnName("momId");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.ActionItems)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK__ActionIte__assig__59FA5E80");

            entity.HasOne(d => d.Mom).WithMany(p => p.ActionItems)
                .HasForeignKey(d => d.MomId)
                .HasConstraintName("FK__ActionIte__momId__59063A47");
        });

        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Attachme__3214EC070EA14F19");

            entity.HasIndex(e => e.MomId, "IX_Attachments_momId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.FileName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("fileName");
            entity.Property(e => e.FilePath)
                .IsUnicode(false)
                .HasColumnName("filePath");
            entity.Property(e => e.MomId).HasColumnName("momId");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Mom).WithMany(p => p.Attachments)
                .HasForeignKey(d => d.MomId)
                .HasConstraintName("FK__Attachmen__momId__5EBF139D");
        });

        modelBuilder.Entity<Feature>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Features__3214EC07376EA81E");

            entity.HasIndex(e => e.FeatureName, "UQ__Features__9C535D4358A42CF9").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.FeatureName)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("featureName");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
        });

        modelBuilder.Entity<Meeting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Meetings__3214EC076699A06C");

            entity.HasIndex(e => e.CreatedBy, "IX_Meetings_createdBy");

            entity.HasIndex(e => e.RoomId, "IX_Meetings_roomId");

            entity.HasIndex(e => e.Title, "UQ__Meetings__E52A1BB36FC2F9C7").IsUnique();

            entity.Property(e => e.Agenda)
                .IsUnicode(false)
                .HasColumnName("agenda");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.CreatedBy).HasColumnName("createdBy");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("endTime");
            entity.Property(e => e.RoomId).HasColumnName("roomId");
            entity.Property(e => e.StartTime)
                .HasColumnType("datetime")
                .HasColumnName("startTime");
            entity.Property(e => e.Status)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.MeetingsNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Meetings__create__5070F446");

            entity.HasOne(d => d.Room).WithMany(p => p.Meetings)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Meetings__roomId__4E88ABD4");

            entity.HasMany(d => d.Users).WithMany(p => p.Meetings)
                .UsingEntity<Dictionary<string, object>>(
                    "Attendee",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__Attendees__userI__6383C8BA"),
                    l => l.HasOne<Meeting>().WithMany()
                        .HasForeignKey("MeetingId")
                        .HasConstraintName("FK__Attendees__meeti__628FA481"),
                    j =>
                    {
                        j.HasKey("MeetingId", "UserId").HasName("PK__Attendee__B0E7CFCB369DBC68");
                        j.ToTable("Attendees");
                        j.HasIndex(new[] { "MeetingId" }, "IX_Attendees_meetingId");
                        j.HasIndex(new[] { "UserId" }, "IX_Attendees_userId");
                        j.IndexerProperty<int>("MeetingId").HasColumnName("meetingId");
                        j.IndexerProperty<int>("UserId").HasColumnName("userId");
                    });
        });

        modelBuilder.Entity<Mom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Moms__3214EC07543EDA02");

            entity.HasIndex(e => e.MeetingId, "IX_Moms_meetingId");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Decisions)
                .IsUnicode(false)
                .HasColumnName("decisions");
            entity.Property(e => e.MeetingId).HasColumnName("meetingId");
            entity.Property(e => e.Notes)
                .IsUnicode(false)
                .HasColumnName("notes");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Meeting).WithMany(p => p.Moms)
                .HasForeignKey(d => d.MeetingId)
                .HasConstraintName("FK__Moms__meetingId__5535A963");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07F39DB56F");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__B1947861B3CFB9C6").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("roleName");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rooms__3214EC0788B27987");

            entity.HasIndex(e => e.Name, "UQ__Rooms__72E12F1B32E864F2").IsUnique();

            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Location)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasMany(d => d.Features).WithMany(p => p.Rooms)
                .UsingEntity<Dictionary<string, object>>(
                    "RoomFeature",
                    r => r.HasOne<Feature>().WithMany()
                        .HasForeignKey("FeatureId")
                        .HasConstraintName("FK__RoomFeatu__featu__4AB81AF0"),
                    l => l.HasOne<Room>().WithMany()
                        .HasForeignKey("RoomId")
                        .HasConstraintName("FK__RoomFeatu__roomI__49C3F6B7"),
                    j =>
                    {
                        j.HasKey("RoomId", "FeatureId").HasName("PK__RoomFeat__4374E43586401221");
                        j.ToTable("RoomFeatures");
                        j.HasIndex(new[] { "FeatureId" }, "IX_RoomFeatures_featureId");
                        j.HasIndex(new[] { "RoomId" }, "IX_RoomFeatures_roomId");
                        j.IndexerProperty<int>("RoomId").HasColumnName("roomId");
                        j.IndexerProperty<int>("FeatureId").HasColumnName("featureId");
                    });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0712051949");

            entity.HasIndex(e => e.RoleId, "IX_Users_roleId");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E616474CBD637").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("passwordHash");
            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__roleId__3D5E1FD2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
