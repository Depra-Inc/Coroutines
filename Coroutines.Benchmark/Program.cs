// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using BenchmarkDotNet.Validators;

BenchmarkRunner.Run(typeof(Program).Assembly, DefaultConfig.Instance
	.AddValidator(JitOptimizationsValidator.FailOnError)
	.AddJob(Job.Default.WithToolchain(InProcessEmitToolchain.Instance))
	.AddDiagnoser(MemoryDiagnoser.Default)
	.WithOrderer(new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest)));