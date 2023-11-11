using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItSoftware.Core.Exception
{
    public class ItsExceptionRenderExtension
    {
        public static List<ItsExceptionRenderExtension> RenderExtensions { get; } = new List<ItsExceptionRenderExtension>();
        static ItsExceptionRenderExtension()
        {
            //
            // DbEntityValidationException
            //
            ItsExceptionRenderExtension.RenderExtensions.Add(new ItsExceptionRenderExtension()
            {
                FullName = "System.Data.Entity.Validation.DbEntityValidationException",
                Header = "DbEntityValidationException",
                Properties = new ItsExceptionRenderPropertyExtension[] {
                    new ItsExceptionRenderPropertyExtension() {
                        Name = "EntityValidationErrors",
                        IsEnumerable = true,
                        Properties = new ItsExceptionRenderPropertyExtension[] {
                            new ItsExceptionRenderPropertyExtension() {
                                Name = "ValidationErrors",
                                IsEnumerable = true,
                                Properties = new ItsExceptionRenderPropertyExtension[] {
                                    new ItsExceptionRenderPropertyExtension() {
                                        Name = "PropertyName",
                                        IsEnumerable = false,
                                    },
                                    new ItsExceptionRenderPropertyExtension() {
                                        Name = "ErrorMessage",
                                        IsEnumerable = false,
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }
        public static string Render(System.Exception x)
        {
            if (ItsExceptionRenderExtension.RenderExtensions.Count == 0 )
			{
                return string.Empty;
			}

            var output = new StringBuilder();
            output.AppendLine();

            try
            {
                foreach (var render in ItsExceptionRenderExtension.RenderExtensions)
                {
                    var outx = new StringBuilder();
                    outx.AppendLine("#####################################");
                    outx.AppendLine($"## {render.Header}");
                    outx.AppendLine("##");

                    var bClear = true;
                    var t = x.GetType();
                    if (t.FullName == render.FullName)
                    {
                        foreach (var p in render.Properties)
                        {
                            outx.Append(ItsExceptionRenderExtension.RenderProperty(x, t, p, x));
                            bClear = false;
                        }
                    }

                    if (!bClear)
                    {
                        output.Append(outx.ToString());
                    }
                }
            }
            catch ( System.Exception y )
            {
                output.Append(y.ToString());
            }

            return output.ToString();
        }
        private static string RenderProperty(System.Exception x, Type t, ItsExceptionRenderPropertyExtension prop, object obj)
        {
            var output = new StringBuilder();

            if (prop.IsEnumerable)
            {
                var propp = t.GetProperty(prop.Name);
                if ( propp == null )
				{
                    return string.Empty;
				}
                var propv = propp.GetValue(obj);
                if ( propv == null )
				{
                    return string.Empty;
				}
                var propie = propv as IEnumerable;
                if ( propie == null )
				{
                    return string.Empty;
				}
                int i = 0;
                foreach (var pie in propie)
                {
                    if (prop.Properties != null && prop.Properties.Length > 0)
                    {
                        foreach (var p in prop.Properties)
                        {
                            output.Append(ItsExceptionRenderExtension.RenderProperty(x, pie.GetType(), p, pie));
                        }
                    }
                    else
                    {
                        output.AppendLine($"{prop.Name}[{i}] = {pie}");
                    }
                    i++;
                }
            }
            else
            {
                var propv = t.GetProperty(prop.Name);
                if ( propv == null )
				{
                    return string.Empty;
				}
                var val = propv.GetValue(obj);
                if ( val == null )
				{
                    return string.Empty;
				}
                output.AppendLine($"{prop.Name} = {val}");

                if (prop.Properties != null)
                {
                    foreach (var p in prop.Properties)
                    {
                        output.Append(ItsExceptionRenderExtension.RenderProperty(x, t, p, obj));
                    }
                }
            }

            return output.ToString();
        }

        public string FullName { get; set; } = string.Empty;
        public string Header { get; set; } = string.Empty;
        public ItsExceptionRenderPropertyExtension[] Properties { get; set; } = null;
    }
}
