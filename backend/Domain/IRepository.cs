// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

namespace Domain;

public interface IRepository
{
    public Result Peek(Identifier id);
    public Result Create(Identifier id, Secret secret);
    public (Result result, Secret? secret) Read(Identifier id);
    public Result Delete(Identifier id);
}
