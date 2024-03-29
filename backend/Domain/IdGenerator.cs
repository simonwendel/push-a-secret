﻿// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

namespace Domain;

internal class IdGenerator : IIdGenerator
{
    private readonly ITimestampGenerator timestampGenerator;
    private readonly IBaseConverter baseConverter;

    public IdGenerator(ITimestampGenerator timestampGenerator, IBaseConverter baseConverter)
    {
        this.timestampGenerator = timestampGenerator;
        this.baseConverter = baseConverter;
    }

    public Identifier Generate()
    {
        var timestamp = timestampGenerator.Generate();
        var base36 = baseConverter.ToBase36(timestamp);
        return new Identifier(base36);
    }

    public static IIdGenerator Default =>
        new IdGenerator(new TimestampGenerator(), new BaseConverter());
}
