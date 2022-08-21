// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

namespace Domain;

public enum Result
{
    OK,
    Err
}

public record Identifier(string Value);

public record Secret(string Algorithm, string IV, string Ciphertext, int Ttl);

public record IdentifierResult(Result Result, Identifier? Identifier);

public record SecretResult(Result Result, Secret? Secret);
