using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.Commons.Models;

public record AccessTokenResult(string AccessToken, DateTimeOffset Expires);