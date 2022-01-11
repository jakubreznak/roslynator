﻿// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class AnalyzerOptionIsObsoleteAnalyzer : AbstractAnalyzerOptionIsObsoleteAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                {
                    Immutable.InterlockedInitialize(
                        ref _supportedDiagnostics,
                        DiagnosticRules.AddOrRemoveAccessibilityModifiers,
                        DiagnosticRules.AddOrRemoveParenthesesFromConditionInConditionalOperator,
                        DiagnosticRules.ConfigureAwait,
                        DiagnosticRules.NormalizeNullCheck,
                        DiagnosticRules.RemoveUnusedMemberDeclaration,
                        DiagnosticRules.UseAnonymousFunctionOrMethodGroup,
                        DiagnosticRules.UseBlockBodyOrExpressionBody,
                        DiagnosticRules.UseEmptyStringLiteralOrStringEmpty,
                        DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray,
                        DiagnosticRules.UseHasFlagMethodOrBitwiseOperator,
                        DiagnosticRules.UseImplicitOrExplicitObjectCreation,
                        CommonDiagnosticRules.AnalyzerOptionIsObsolete);
                }

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

            context.RegisterCompilationStartAction(compilationContext =>
            {
                var flags = Flags.None;

                CompilationOptions compilationOptions = compilationContext.Compilation.Options;

                compilationContext.RegisterSyntaxTreeAction(context =>
                {
                    if (!CommonDiagnosticRules.AnalyzerOptionIsObsolete.IsEffective(context.Tree, compilationOptions, context.CancellationToken))
                        return;

                    AnalyzerConfigOptions options = context.GetConfigOptions();

                    Validate(ref context, compilationOptions, options, Flags.ConvertBitwiseOperationToHasFlagCall, ref flags, DiagnosticRules.UseHasFlagMethodOrBitwiseOperator, ConfigOptions.EnumHasFlagStyle, LegacyConfigOptions.ConvertBitwiseOperationToHasFlagCall, ConfigOptionValues.EnumHasFlagStyle_Method);
                    Validate(ref context, compilationOptions, options, Flags.ConvertExpressionBodyToBlockBody, ref flags, DiagnosticRules.UseBlockBodyOrExpressionBody, ConfigOptions.BodyStyle, LegacyConfigOptions.ConvertExpressionBodyToBlockBody, ConfigOptionValues.BodyStyle_Block);
                    Validate(ref context, compilationOptions, options, Flags.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine, ref flags, DiagnosticRules.UseBlockBodyOrExpressionBody, ConfigOptions.BodyStyle, LegacyConfigOptions.ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine, "true");
                    Validate(ref context, compilationOptions, options, Flags.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine, ref flags, DiagnosticRules.UseBlockBodyOrExpressionBody, ConfigOptions.BodyStyle, LegacyConfigOptions.ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine, "true");
                    Validate(ref context, compilationOptions, options, Flags.ConvertMethodGroupToAnonymousFunction, ref flags, DiagnosticRules.UseAnonymousFunctionOrMethodGroup, ConfigOptions.AnonymousFunctionOrMethodGroup, LegacyConfigOptions.ConvertMethodGroupToAnonymousFunction, ConfigOptionValues.AnonymousFunctionOrMethodGroup_AnonymousFunction);
                    Validate(ref context, compilationOptions, options, Flags.RemoveCallToConfigureAwait, ref flags, DiagnosticRules.ConfigureAwait, ConfigOptions.ConfigureAwait, LegacyConfigOptions.RemoveCallToConfigureAwait, "false");
                    Validate(ref context, compilationOptions, options, Flags.RemoveAccessibilityModifiers, ref flags, DiagnosticRules.AddOrRemoveAccessibilityModifiers, ConfigOptions.AccessibilityModifiers, LegacyConfigOptions.RemoveAccessibilityModifiers, ConfigOptionValues.AccessibilityModifiers_Implicit);
                    Validate(ref context, compilationOptions, options, Flags.RemoveEmptyLineBetweenClosingBraceAndSwitchSection, ref flags, DiagnosticRules.RemoveUnnecessaryBlankLine, ConfigOptions.BlankLineBetweenClosingBraceAndSwitchSection, LegacyConfigOptions.RemoveEmptyLineBetweenClosingBraceAndSwitchSection, "false");
                    Validate(ref context, compilationOptions, options, Flags.RemoveParenthesesFromConditionOfConditionalExpressionWhenExpressionIsSingleToken, ref flags, DiagnosticRules.AddOrRemoveParenthesesFromConditionInConditionalOperator, ConfigOptions.ConditionInConditionalOperatorParenthesesStyle, LegacyConfigOptions.RemoveParenthesesFromConditionOfConditionalExpressionWhenExpressionIsSingleToken, ConfigOptionValues.ConditionInConditionalExpressionParenthesesStyle_OmitWhenConditionIsSingleToken);
                    Validate(ref context, compilationOptions, options, Flags.RemoveParenthesesWhenCreatingNewObject, ref flags, DiagnosticRules.UseImplicitOrExplicitObjectCreation, ConfigOptions.ObjectCreationParenthesesStyle, LegacyConfigOptions.RemoveParenthesesWhenCreatingNewObject, ConfigOptionValues.ObjectCreationParenthesesStyle_Omit);
                    Validate(ref context, compilationOptions, options, Flags.SuppressUnityScriptMethods, ref flags, DiagnosticRules.RemoveUnusedMemberDeclaration, ConfigOptions.SuppressUnityScriptMethods, LegacyConfigOptions.SuppressUnityScriptMethods, "true");
                    Validate(ref context, compilationOptions, options, Flags.UseComparisonInsteadPatternMatchingToCheckForNull, ref flags, DiagnosticRules.NormalizeNullCheck, ConfigOptions.NullCheckStyle, LegacyConfigOptions.UseComparisonInsteadPatternMatchingToCheckForNull, ConfigOptionValues.NullCheckStyle_EqualityOperator);
                    Validate(ref context, compilationOptions, options, Flags.UseImplicitlyTypedArray, ref flags, DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray, ConfigOptions.ArrayCreationTypeStyle, LegacyConfigOptions.UseImplicitlyTypedArray, ConfigOptionValues.ArrayCreationTypeStyle_Implicit);
                    Validate(ref context, compilationOptions, options, Flags.UseImplicitlyTypedArrayWhenTypeIsObvious, ref flags, DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray, ConfigOptions.ArrayCreationTypeStyle, LegacyConfigOptions.UseImplicitlyTypedArrayWhenTypeIsObvious, ConfigOptionValues.ArrayCreationTypeStyle_ImplicitWhenTypeIsObvious);
                    Validate(ref context, compilationOptions, options, Flags.UseStringEmptyInsteadOfEmptyStringLiteral, ref flags, DiagnosticRules.UseEmptyStringLiteralOrStringEmpty, ConfigOptions.EmptyStringStyle, LegacyConfigOptions.UseStringEmptyInsteadOfEmptyStringLiteral, ConfigOptionValues.EmptyStringStyle_Field);
                });
            });
        }

        private static void Validate(
            ref SyntaxTreeAnalysisContext context,
            CompilationOptions compilationOptions,
            AnalyzerConfigOptions configOptions,
            Flags flag,
            ref Flags flags,
            DiagnosticDescriptor analyzer,
            ConfigOptionDescriptor option,
            LegacyConfigOptionDescriptor legacyOption,
            string newValue)
        {
            if (!flags.HasFlag(flag)
                && analyzer.IsEffective(context.Tree, compilationOptions, context.CancellationToken)
                && TryReportObsoleteOption(context, configOptions, legacyOption, option, newValue))
            {
                flags |= flag;
            }
        }

        [Flags]
        private enum Flags
        {
            None,
            ConvertBitwiseOperationToHasFlagCall,
            ConvertExpressionBodyToBlockBody,
            ConvertExpressionBodyToBlockBodyWhenDeclarationIsMultiLine,
            ConvertExpressionBodyToBlockBodyWhenExpressionIsMultiLine,
            ConvertMethodGroupToAnonymousFunction,
            RemoveAccessibilityModifiers,
            RemoveCallToConfigureAwait,
            RemoveEmptyLineBetweenClosingBraceAndSwitchSection,
            RemoveParenthesesFromConditionOfConditionalExpressionWhenExpressionIsSingleToken,
            RemoveParenthesesWhenCreatingNewObject,
            SuppressUnityScriptMethods,
            UseComparisonInsteadPatternMatchingToCheckForNull,
            UseImplicitlyTypedArray,
            UseImplicitlyTypedArrayWhenTypeIsObvious,
            UseStringEmptyInsteadOfEmptyStringLiteral,
        }
    }
}
