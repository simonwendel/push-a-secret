// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

namespace Domain;

public interface IStore
{
    Result Peek(Identifier identifier);
    IdentifierResult Create(Secret secret);
    SecretResult Read(Identifier identifier);
    Result Delete(Identifier identifier);
}
