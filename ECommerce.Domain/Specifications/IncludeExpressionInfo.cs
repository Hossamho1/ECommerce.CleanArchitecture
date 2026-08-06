using System;
using System.Collections.Generic;
using System.Text;

using System.Linq.Expressions;

namespace ECommerce.Domain.Specifications;


public sealed record IncludeExpressionInfo(
    LambdaExpression LambdaExpression,
    LambdaExpression PreviousExpression
);