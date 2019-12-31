using System;
using MicroFeel.CLS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace MicroFeel.Logging.Extensions
{
    public static class ClsLoggerExtensions
    {
        public static ILoggingBuilder AddCls(this ILoggingBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            builder.Services.TryAddSingleton(ServiceDescriptor.Singleton<ILoggerProvider, ClsLoggerProvider>());
            return builder;
        }

        public static ILoggingBuilder AddCls(this ILoggingBuilder builder, ClsSetting settings)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (settings.Enabled)
            {
                builder.Services.AddSingleton<ILoggerProvider>(new ClsLoggerProvider(settings));
            }

            return builder;
        }

        /// <summary>
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <param name="configure">The settings to apply to created <see cref="ClsSetting"/>'s.</param>
        /// <returns></returns>
        public static ILoggingBuilder AddCls(this ILoggingBuilder builder, Action<ClsSetting> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddCls();
            builder.Services.Configure(configure);
            return builder;
        }

    }
}