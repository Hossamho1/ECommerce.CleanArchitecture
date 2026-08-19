using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Commons.Models;

public sealed record AuthUserSnapshot(Guid UserId, string Email, string? DisplayName);