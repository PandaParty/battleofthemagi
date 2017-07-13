using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

public static class BotmUtil
{
    public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
    {
        MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
        return expressionBody.Member.Name;
    }
}
