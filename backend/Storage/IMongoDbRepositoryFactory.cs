// SPDX-FileCopyrightText: 2022 Simon Wendel
// SPDX-License-Identifier: GPL-3.0-or-later

namespace Storage;

internal interface IMongoDbRepositoryFactory
{
    MongoDbRepository Build();
}
