using System;
using System.Collections.Generic;

namespace prjSpecialTopicWebAPI.Models;

public partial class UsedBookImage
{
    public int Id { get; set; }

    public Guid BookId { get; set; }

    public bool IsCover { get; set; }

    public int DisplayOrder { get; set; }

    public byte StorageProvider { get; set; }

    public string ObjectKey { get; set; } = null!;

    public byte[] Sha256 { get; set; } = null!;

    public DateTime UploadedAt { get; set; }

    public virtual UsedBook Book { get; set; } = null!;
}
